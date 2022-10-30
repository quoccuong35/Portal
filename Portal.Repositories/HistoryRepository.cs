using Portal.EntityModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Repositories
{
    public interface IRepository
    {
        void SaveUpdateHistory<T>(Guid Id, string UserName, T obj, params Expression<Func<T, object>>[] propertiesToUpdate) where T : class;
    }
    public class HistoryRepository : IRepository
    {
        PortalEntities _context;
        public HistoryRepository(PortalEntities ct)
        {
            _context = ct;
        }

        public const string LastModifiedTime = "LastModifiedTime";

        public void SaveUpdateHistory<T>(Guid Id, string UserName, T obj, params Expression<Func<T, object>>[] propertiesToUpdate) where T : class
        {
            //Ensure only modified fields are updated.
            var dbEntityEntry = _context.Entry(obj);
            string tableName = dbEntityEntry.Entity.GetType().Name;
            if (propertiesToUpdate.Any())
            {
                //update explicitly mentioned properties
                foreach (var property in propertiesToUpdate)
                {
                    dbEntityEntry.Property(property).IsModified = true;
                }
            }
            else
            {
                //no items mentioned, so find out the updated entries
                var dateTime = DateTime.Now;
                foreach (var property in dbEntityEntry.OriginalValues.PropertyNames)
                {
                    var original = dbEntityEntry.OriginalValues.GetValue<object>(property);
                    var current = dbEntityEntry.CurrentValues.GetValue<object>(property);
                    if (original != null && !original.Equals(current) && property != LastModifiedTime)
                    {
                        //dbEntityEntry.Property(property).IsModified = true;

                        //NOTE: MUST USING VIEW MODEL
                        #region Insert into HistoryModel
                        HistoryModel model = new HistoryModel();
                        model.HistoryModifyId = Guid.NewGuid();
                        //get PageId
                        var path = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
                        Guid? PageId = _context.PageModels.Where(p => path.Contains(p.PageUrl))
                                                       .Select(p => p.PageId).FirstOrDefault();
                        model.PageId = (PageId == Guid.Empty) ? null : PageId;
                        model.FieldId = Id;
                        model.FieldName = property;
                        model.OldData = original != null ? original.ToString():"";
                        model.NewData = current!=null?current.ToString():"";
                        model.TableName = tableName;
                        model.LastModifiedUser = UserName;
                        model.LastModifiedTime = dateTime;
                        _context.Entry(model).State = EntityState.Added;
                        #endregion Insert into HistoryModel
                    }
                }
            }
        }
    }
}
