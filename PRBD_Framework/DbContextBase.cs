using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace PRBD_Framework {
    internal class DbHistory {
        [Key]
        public string TableName { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now.ToUniversalTime();
    }

    public class DbContextBase : DbContext {
        internal DbSet<DbHistory> DbHistories { get; set; }
        private static Dictionary<string, DateTime> history = new Dictionary<string, DateTime>();

        public DbContextBase() : base() { }

        public DbContextBase(DbContextOptions options) : base(options) {
        }

        public bool NeedsRefreshData() {
            try {
                if (ChangeTracker.HasChanges())
                    return false;
                var histEntries = (from h in DbHistories select h).ToList();
                bool needsRefresh = false;
                foreach (var h in histEntries) {
                    var t = history.ContainsKey(h.TableName) ? history[h.TableName] : new DateTime();
                    Entry(h).Reload();
                    if (h.Timestamp != t) {
                        needsRefresh = true;
                        history[h.TableName] = h.Timestamp;
                    }
                }
                return needsRefresh;
            } catch (Exception) {
                return true;
            }
        }

        private void UpdateHistory() {
            var tables = ((from e in ChangeTracker.Entries()
                           where e.State != EntityState.Unchanged
                           select e.Metadata.GetTableName()).Distinct()).ToList();
            foreach (var table in tables) {
                var hist = DbHistories.Find(table);
                if (hist == null) {
                    hist = new DbHistory { TableName = table };
                    DbHistories.Add(hist);
                }
                hist.Timestamp = DateTime.Now.ToUniversalTime();
                history[table] = hist.Timestamp;
            }
        }

        public override int SaveChanges() {
            int count = -1;
            bool hasTransaction = Database.CurrentTransaction != null;
            try {
                if (ExecuteValidation()) {
                    // s'il n'y a pas de transaction en cours, en créer une
                    if (!hasTransaction)
                        Database.BeginTransaction();
                    // mettre à jour la table historique pour les entités impactées
                    UpdateHistory();
                    count = base.SaveChanges();
                    if (!hasTransaction)
                        Database.CommitTransaction();
                    return count;
                }
                Console.WriteLine("SaveChanges() not successful due to business rules errors");
            } catch (DbUpdateConcurrencyException ex) {
                Console.WriteLine("SaveChanges() not successful due to optimistic locking violation");
                // see: https://docs.microsoft.com/en-us/ef/core/saving/concurrency
                foreach (var entry in ex.Entries) {
                    entry.Reload();
                }
                ApplicationBase.NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
            }
            if (!hasTransaction)
                Database.RollbackTransaction();
            return count;
        }

        private bool ExecuteValidation() {
            bool hasErrors = false;
            foreach (var entry in ChangeTracker.Entries()
                .Where(e => (e.State == EntityState.Added) || (e.State == EntityState.Modified)).ToList()) {
                if (entry.Entity is IErrorManager) {
                    var entity = entry.Entity as IErrorManager;
                    entity.Validate();
                    if (entity.HasErrors) {
                        Console.WriteLine($"Business rules errors in entity of type {entity.GetType().Name}:\n" +
                            JsonConvert.SerializeObject(entity.Errors, Formatting.Indented));
                        hasErrors = true;
                    }
                }
            }
            return !hasErrors;
        }

        public void Reload(object entity) {
            Entry(entity)?.Reload();
        }

        public void Detach(object entity) {
            var entry = Entry(entity);
            if (entity == null) return;
            entry.State = EntityState.Detached;
        }
    }
}
