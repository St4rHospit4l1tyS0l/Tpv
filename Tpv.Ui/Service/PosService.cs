/*using System;
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
                _empInfo = emp.GetStringVal("FIRSTNAME") + " "  + emp.GetStringVal("LASTNAME");
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