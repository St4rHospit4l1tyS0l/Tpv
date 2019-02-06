using LasaFOHLib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using Tpv.Printer.Infrastructure.Log;
using Tpv.Printer.Model.Shared;

namespace Tpv.Printer.Repository
{
    public static class DbfDataRepository
    {
        public static List<int> GetItemsIdsByCategory()
        {
            try
            {
                if (MasterModel.CategoryIds == null || MasterModel.CategoryIds.Count == 0)
                    return new List<int>();

                var lstItemsIds = new List<int>();
                var depot = new IberDepotClass();
                foreach (IberObject cat in depot.GetEnum(Constants.InternalPos.FILE_CAT))
                {
                    try
                    {
                        var catId = cat.GetLongVal("ID");
                        if (!MasterModel.CategoryIds.Contains(catId)) continue;

                        foreach (IberObject itm in cat.GetEnum(Constants.InternalPos.INTERNAL_CATS_ITEMIDS))
                        {
                            try
                            {
                                lstItemsIds.Add(itm.GetLongVal("ID"));
                            }
                            catch (Exception ex)
                            {
                                Logger.Log(ex);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }

                return lstItemsIds.Distinct().ToList();

            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return null;
            }
        }

        public static Dictionary<int, string> GetItems()
        {
            var dicItems = new Dictionary<int, string>();

            try
            {
                var depot = new IberDepotClass();
                foreach (IberObject itm in depot.GetEnum(Constants.InternalPos.FILE_ITM))
                {
                    try
                    {
                        var id = itm.GetLongVal("ID");

                        if (dicItems.ContainsKey(id))
                            continue;

                        var longName = itm.GetStringVal("LONGNAME");
                        dicItems.Add(id, string.IsNullOrWhiteSpace(longName) ? string.Empty : longName.ToUpperInvariant());

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }

                return dicItems;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return null;
            }
        }

        public static List<int> GetItemsIdsByCategoryFromFile()
        {
            try
            {
                var query = Constants.Database.CAT_ITEM_QUERY;

                query = $"{query} WHERE CIT.CATEGORY IN ({string.Join(", ", MasterModel.CategoryIds)})";

                var dt = DoQuery(query);

                int value;
                return dt.AsEnumerable().Select(e =>
                {
                    int.TryParse(e["ITEMID"].ToString(), out value);
                    return value;
                }).Distinct().ToList();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return null;
            }
        }

        private static DataTable DoQuery(string query)
        {
            var dt = new DataTable();
            using (var dbConn = new OleDbConnection(GlobalParams.ConnStr))
            {
                using (var da = new OleDbDataAdapter(query, dbConn))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }

    }
}
