
using LasaFOHLib67;
using log4net;
using System;
using Tpv.Ui.Model;

namespace Tpv.Ui.Service
{
    public static class PosService
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(PosService));

        public const int INTERNAL_CHECKS = 540;
        public const int INTERNAL_CHECKS_ENTRIES = 542;
        public const int INTERNAL_ENTRIES_ITEM_DATA = 562;
        public const int INTERNAL_CHECKS_PROMOS = 544;
        public const int INTERNAL_PROMOS_ITEMS = 621;


        private static bool GetInternalItems(IberDepot pDepot, PosCheckModel model)
        {
            try
            {
                foreach (IIberObject chkObject in pDepot.FindObjectFromId(INTERNAL_CHECKS, model.CheckId))
                {
                    GetPromosIfAny(chkObject, model);
                    model.Pvp = (decimal)chkObject.GetDoubleVal("COMPLETETOTAL");
                }
                return true;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message + " | " + ex.StackTrace);
                MessageExt.ShowErrorMessage("El ticket no pudo ser leido, revise que esté activo.");
                return false;
            }
        }

        private static void GetPromosIfAny(IIberObject chkObject, PosCheckModel model)
        {
            decimal pvpPromo = 0;
            try
            {
                foreach (IIberObject objInternal in chkObject.GetEnum(INTERNAL_CHECKS_PROMOS))
                {
                    pvpPromo += (decimal)objInternal.GetDoubleVal("AMOUNT");
                }
            }
            catch
            {
                //return null;
            }

            model.PvpPromo = pvpPromo;
        }

        public static int GetReadableCheckId(this int checkId)
        {
            long decodeTerm = checkId >> 20;
            long decodeRel = checkId & 0xFFFFF;
            return Convert.ToInt32(decodeTerm * 10000 + decodeRel);
        }

        public static bool ReadCheckInfo(PosCheckModel model)
        {

            try
            {
                var depot = new IberDepotClass();
                IIberObject localState = depot.GetEnum(720).First();

                try
                {
                    model.TermId = localState.GetLongVal("TERMINAL_NUM");
                    model.CheckId = localState.GetLongVal("CURRENT_CHECK_ID");
                    model.ReadableCheckId = GetReadableCheckId(model.CheckId);
                    return GetInternalItems(depot, model);
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message + " | " + ex.StackTrace);
                    MessageExt.ShowErrorMessage("Asegure que el TPV está operando.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message + " | " + ex.StackTrace);
                MessageExt.ShowErrorMessage("Asegure que el TPV está operando o tenga la licencia necesaria.");
                return false;
            }
        }
    }
}

/*
 * 
 * 

    /*
                    foreach (IIberObject objItem in chkObject.GetEnum(INTERNAL_CHECKS_ENTRIES))
                    {
                        
                        var modCode = objItem.GetLongVal("MOD_CODE");
                        if (modCode == 8) continue; //ITEM DELETED

                        var idItem = objItem.GetLongVal("DATA");
                        if (idItem <= 0) continue;

                        //var type = objItem.GetStringVal("TYPE");
                        //var mode = objItem.GetStringVal("MODE");
                        //var routing = objItem.GetStringVal("ROUTING");
                        //var menu = objItem.GetStringVal("MENU");
                        var origin = objItem.GetLongVal("ORIGIN");

                        var idCheckItem = objItem.GetLongVal("ID");
                        var itemName = objItem.GetStringVal("DISP_NAME");
                        var price = objItem.GetDoubleVal("PRICE");
                        var level = objItem.GetLongVal("LEVEL");
                        var item = new ItemModel
                        {
                            ItemId = idItem,
                            CheckItemId = idCheckItem,
                            Name = itemName,
                            IsIdSpecified = true,
                            Price = price,
                            Level = level,
                            ModCode = modCode,
                            Origin = origin
                        };

                        //Console.WriteLine(type + mode + origin + routing + menu);

                        dictLevels[level] = item;

                        if (level > 0)
                            item.Parent = dictLevels[level - 1];

                        lstItems.Add(item);
                }*/
/*
using System;
using System.Windows;

namespace Tpv.Ui.Service
{
public class PosService
{
    private int _termId;
    private int _checkId;
    private string _empInfo;
    //private int _tableId;

    public bool ApplyPromotion(int iCode)
    {
        LasaFOHLib67.IberDepot depot = new LasaFOHLib67.IberDepotClass();
        LasaFOHLib67.IIberObject localState = depot.GetEnum(720).First();

        try
        {
            LasaFOHLib67.IIberObject emp = localState.GetEnum(723).First();
            _empInfo = emp.GetStringVal("FIRSTNAME") + " " + emp.GetStringVal("LASTNAME");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            MessageBox.Show("Make sure someone is logged into the FOH.");
            return false;
        }

        try
        {
            _termId = localState.GetLongVal("TERMINAL_NUM");
            _checkId = localState.GetLongVal("CURRENT_CHECK_ID");
            LasaFOHLib67.IIberObject check = depot.FindObjectFromId(540, _checkId).First();
            //_tableId = check.GetLongVal("TABLE_ID");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }

        LasaFOHLib67.IberFuncs funcs;
        try
        {
            funcs = new LasaFOHLib67.IberFuncsClass();
            //funcs.OrderItems(_termId, _tableId, 1);
            funcs.RefreshCheckDisplay();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }

        try
        {
            funcs.DeselectAllEntries(_termId);
            //funcs.BeginItem(_termId, _checkId, iCode, "", -1000000000.00);
            var entry = funcs.EnterItem(_checkId, iCode, 1.8);
            Console.WriteLine(entry);
            //funcs.ApplyPromo(_termId, 0, _checkId, 1, 0.00, ""); //iCode
            //funcs.RefreshCheckDisplay();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }

        return true;
    }
}
}
*/
