using AdmInterceptActivity;
using AlohaFOHLib.Intl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using Tpv.Aloha.Infrastructure.Connector;
using Tpv.Aloha.Model;
using Tpv.Aloha.Service;

namespace Tpv.Aloha.BsLogic
{
    public class PosActService : IPosActService
    {
        public const int INTERNAL_CHECKS = 540;
        public const int INTERNAL_CHECKS_ENTRIES = 542;
        public const int INTERNAL_ENTRIES_ITEM_DATA = 562;

        private void GetInternalItems(int iCheckId)
        {
            var pDepot = new IberDepot();
            try
            {
                //var posCheck = new PosCheck();
                //var sItems = String.Empty;
                //var lstItems = new List<ItemModel>();

                foreach (IIberObject chkObject in pDepot.FindObjectFromId(INTERNAL_CHECKS, iCheckId))
                {
                    foreach (IIberObject entryObject in chkObject.GetEnum(INTERNAL_CHECKS_ENTRIES))
                    {
                        foreach (IIberObject objItem in entryObject.GetEnum(INTERNAL_ENTRIES_ITEM_DATA))
                        {
                            var idItem = objItem.GetLongVal("ID");
                            var itemName = objItem.GetStringVal("LONGNAME");
                            var price = objItem.GetDoubleVal("PRICE");
                            ProcessItemIfPromo(idItem, itemName, price, iCheckId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message + " - " + ex.StackTrace);
            }
        }

        private void ProcessItemIfPromo(int idItem, string itemName, double price, int iCheckId)
        {
            if (DbReader.DicPromos.ContainsKey(idItem) == false)
                return;

            Logger.Write(String.Format("INFO - Cheque {0}, PromoId: {1}, PromoName: {2}, Precio: {3}", iCheckId, idItem, itemName, price));

            List<PromoCheckFile> lstItems;
            PromoItemFile promo;
            var barCode = GetBarCodePromo(iCheckId, idItem, out lstItems, out promo);

            if (barCode == null)
                return;

            DeleteBarCodePromo(barCode);

            DeletePromoCheckOrFile(lstItems, iCheckId, promo);
        }

        private void DeletePromoCheckOrFile(List<PromoCheckFile> lstItems, int checkId, PromoItemFile promo)
        {
            for (var i = lstItems.Count - 1; i >= 0; i--)
            {
                var item = lstItems[i];

                if (item.CheckId == checkId)
                    item.LstPromo.Remove(promo);

                for (var j = item.LstPromo.Count - 1; j >= 0; j--)
                {
                    var promoIn = item.LstPromo[j];

                    //5 days in file, then is deleted
                    if ((DateTime.Today - promoIn.Date).Days >= 5)
                        item.LstPromo.RemoveAt(j);
                }

                if (item.LstPromo.Count == 0)
                    lstItems.Remove(item);
            }

            if (lstItems.Count == 0)
            {
                try
                {
                    File.Delete(DbReader.PromoFile);
                }
                catch (Exception ex)
                {
                    Logger.Write("ERROR - No fue posible eliminar el archivo. Intento 1");
                    Logger.Write(ex.Message + " - " + ex.StackTrace);
                    Thread.Sleep(250);
                    try
                    {
                        File.Delete(DbReader.PromoFile);
                    }
                    catch (Exception ex1)
                    {
                        Logger.Write("ERROR - No fue posible eliminar el archivo. Intento 2");
                        Logger.Write(ex1.Message + " - " + ex1.StackTrace);
                    }
                }
            }
            else
            {
                try
                {
                    //Save file
                    var ser = new JavaScriptSerializer().Serialize(lstItems);
                    File.WriteAllText(DbReader.PromoFile, ser);
                }
                catch (Exception ex)
                {
                    Logger.Write("ERROR - No fue guardar en el archivo, la información restante");
                    Logger.Write(ex.Message + " - " + ex.StackTrace);
                }
            }
        }

        private void DeleteBarCodePromo(string barCode)
        {
            var iTries = 3;

            while (iTries-- > 0)
            {
                var resp = RestService.MakeRequest(RestService.CreateRequestToDelete(barCode));

                if (resp == null)
                    continue;

                //Logger.Write("INFO: Al eliminar la promoción, el resultado es: " + resp.Status);
                break;
            }
        }

        private string GetBarCodePromo(int iCheckId, int idItem, out List<PromoCheckFile> lstItems, out PromoItemFile promo)
        {
            lstItems = null;
            promo = null;
            var promoFile = DbReader.PromoFile;
            if (File.Exists(promoFile) == false)
            {
                Logger.Write(String.Format("ERROR - No existe el archivo {0} de promociones para enviar el codigo correspondiente", promoFile));
                return null;
            }

            try
            {
                var fileInfo = File.ReadAllText(promoFile);
                lstItems = new JavaScriptSerializer().Deserialize<List<PromoCheckFile>>(fileInfo);
            }
            catch (Exception ex)
            {
                Logger.Write("ERROR - No fue posible obtener la información del archivo");
                Logger.Write(ex.Message + " - " + ex.StackTrace);

                try
                {
                    File.Delete(promoFile);
                }
                catch (Exception ex1)
                {
                    Logger.Write("ERROR - No fue posible eliminar la información del archivo");
                    Logger.Write(ex1.Message + " - " + ex1.StackTrace);
                    return null;
                }

                return null;
            }

            var item = lstItems.FirstOrDefault(e => e.CheckId == iCheckId);
            if (item == null)
            {
                Logger.Write("ERROR - No hay código en el cheque para esta promoción");
                return null;
            }

            promo = item.LstPromo.FirstOrDefault(e => e.PromoId == idItem);

            if (promo == null)
            {
                Logger.Write("ERROR - La promoción no existe en el archivo.");
                return null;
            }

            return promo.BarCode;
        }

        public void LogOut(int iEmployeeId, string sName)
        {

        }

        public void ClockIn(int iEmployeeId, string sEmpName, int iJobcodeId, string sJobName)
        {

        }

        public void ClockOut(int iEmployeeId, string sEmpName)
        {

        }

        public void OpenTable(int iEmployeeId, int iQueueId, int iTableId, int iTableDefId, string sName)
        {
        }

        public void CloseTable(int iemployeeId, int iqueueId, int itableId)
        {

        }

        public void OpenCheck(int iemployeeId, int iqueueId, int itableId, int icheckId)
        {

        }

        public void CloseCheck(int iemployeeId, int iqueueId, int itableId, int icheckId)
        {
            var fileName = Path.Combine(Environment.CurrentDirectory, Constants.FILE_NAME_CONFIG);
            if (DbReader.ReadDictionaryFromFile(fileName) == false)
            {
                var msg = $"No existe el archivo de configuración {fileName} del TPV o no se pudo leer de forma correcta";
                Logger.Write(msg);
                MessageBox.Show(msg);
            }

            GetInternalItems(icheckId);
        }


        public void TransferTable(int ifromEmployeeId, int itoEmployeeId, int itableId, string sNewName, int iIsGetCheck)
        {

        }

        public void AcceptTable(int iemployeeId, int ifromTableId, int itoTableId)
        {

        }

        public void SaveTab(int iemployeeId, int itableId, string sName)
        {

        }

        public void AddTab(int iemployeeId, int ifromTableId, int itoTableId)
        {

        }

        public void NameOrder(int iemployeeId, int iqueueId, int itableId, string sName)
        {

        }
        public void NameOrder(int iEmployeeId, int iQueueId, int iTableId, string sName, int iCheckId)
        {

        }

        public void Bump(int iTableId)
        {

        }

        public void AddItem(int iEmployeeId, int iQueueId, int iTableId, int iCheckId, int iEntryId)
        {
            //MessageBox.Show("AddItem(): Estamos enviando los items agregados AddItem10: " + iEntryId);
        }

        public void ModifyItem(int iEmployeeId, int iQueueId, int iTableId, int iCheckId, int iEntryId)
        {

        }

        public void OrderItems(int iEmployeeId, int iQueueId, int iTableId, int iCheckId, int iModeId)
        {

        }

        public void HoldItems(int iEmployeeId, int iQueueId, int iTableId, int iCheckId)
        {

        }

        public void OpenItem(int iEmployeeId, int iEntryId, int iItemId, string sDescription, double dPrice)
        {

        }

        public void SpecialMessage(int iEmployeeId, int iMessageId, string sMessage)
        {

        }

        public void DeleteItems(int iManagerId, int iEmployeeId, int iQueueId, int iTableId, int iCheckId, int iReasonId)
        {

        }

        public void UpdateItems(int iEmployeeId, int iQueueId, int iTableId, int iCheckId)
        {

        }

        public void ApplyPayment(int iManagerId, int iEmployeeId, int iQueueId, int iTableId, int iCheckId, int iTenderId, int iPaymentId)
        {

        }

        public void AdjustPayment(int iManagerId, int iEmployeeId, int iQueueId, int iTableId, int iCheckId, int iTenderId, int iPaymentId)
        {

        }

        public void DeletePayment(int iManagerId, int iEmployeeId, int iQueueId, int iTableId, int iCheckId, int iTenderId, int iPaymentId)
        {

        }

        public void ApplyComp(int iManagerId, int iEmployeeId, int iQueueId, int iTableId, int iCheckId, int iCompTypeId, int iCompId)
        {

        }

        public void DeleteComp(int iManagerId, int iEmployeeId, int iQueueId, int iTableId, int iCheckId, int iCompTypeId, int iCompId)
        {

        }

        public void ApplyPromo(int iManagerId, int iEmployeeId, int iQueueId, int iTableId, int iCheckId, int iPromotionId, int iPromoId)
        {

        }

        public void DeletePromo(int iManagerId, int iEmployeeId, int iQueueId, int iTableId, int iCheckId, int iPromotionId, int iPromoId)
        {

        }

        public void Custom(string sName)
        {

        }

        public void Startup(int iHMainWnd)
        {

        }

        public void InitializationComplete()
        {

        }

        public void Shutdown()
        {

        }

        public void CarryoverId(int iType, int iOldId, int iNewId)
        {

        }

        public void EndOfDay(int iIsMaster)
        {

        }

        public void CombineOrder(int iEmployeeId, int iSrcQueueId, int iSrcTableId, int iSrcCheckId, int iDstQueueId, int iDstTableId, int iDstCheckId)
        {

        }

        public void OnClockTick()
        {

        }

        public void PreModifyItem(int iEmployeeId, int iQueueId, int iTableId, int iCheckId, int iEntryId)
        {

        }

        public void LockOrder(int iTableId)
        {

        }

        public void UnlockOrder(int iTableId)
        {

        }

        public void SetMasterTerminal(int iTerminalId)
        {

        }

        public void SetQuickComboLevel(int iEmployeeId, int iQueueId, int iTableId, int iCheckId, int iPromotionId, int iPromoId, int iLevel, int iContext)
        {

        }

        public void TableToShowOnDispBChanged(int iTermId, int iTableId)
        {

        }

        public void StartAddItem(int iEmployeeId, int iQueueId, int iTableId, int iCheckId, int iEntryId, int iParentEntryId, int iModCodeId, int iItemId, string sItemName, double dItemPrice)
        {

        }

        public void CancelAddItem(int iEmployeeId, int iQueueId, int iTableId, int iCheckId, int iEntryId)
        {

        }

        public void PostDeleteItems(int iManagerId, int iEmployeeId, int iQueueId, int iTableId, int iCheckId, int iReasonId)
        {

        }

        public void PostDeleteComp(int iManagerId, int iEmployeeId, int iQueueId, int iTableId, int iCheckId, int iCompTypeId, int iCompId)
        {

        }

        public void PostDeletePromo(int iManagerId, int iEmployeeId, int iQueueId, int iTableId, int iCheckId, int iPromotionId, int iPromoId)
        {

        }

        public void OrderScreenTableCheckSeatChanged(int iManagerId, int iEmployeeId, int iQueueId, int iTableId, int iCheckId, int iSeatNum)
        {

        }

        public void EventNotification(int iEmployeeId, int iTableId, ALOHA_ACTIVITY_EVENT_NOTIFICATION_TYPES eventNotification)
        {

        }

        public void RerouteDisplayBoard(int iEmployeeId, int iQueueId, int iTableId, int iCheckId, int iDisplayBoardId, int iControllingTerminalId, int iDefaultOrderModeOverride, int currentOrderOnly)
        {

        }

        public void ChangeItemSize(int iEmployeeId, int iQueueId, int iTableId, int iCheckId, int iEntryId)
        {

        }

        public void AdvanceOrder(int iEmployeeId, int iQueueId, int iTableId)
        {

        }

        public void EnrollEmployee(int iEmployeeId, int iNumTries)
        {

        }

        public void MasterDown()
        {

        }

        public void AmMaster()
        {

        }

        public void FileServerDown()
        {

        }

        public void FileServer(string sServerName)
        {

        }

        public void SettleInfoChanged(string sSettleInfo)
        {

        }

        public void SplitCheck(int iCheckId, int iTableId, int iQueueId, int iEmployeeNumber, int iNumberOfSplits, int iSplitType)
        {

        }

        public void AuthorizePayment(int iTableId, int iCheckId, int iPaymentId, int iTransactionType, int iTransactionResult)
        {

        }

        public void CurrentCheckChanged(int iTermId, int iTableId, int iCheckId)
        {

        }

        public void FinalBump(int iTableId)
        {

        }

        public void AssignCashDrawer(int iEmployeeId, int iDrawerId, int iIsPublic)
        {

        }

        public void ReassignCashDrawer(int iEmployeeId, int iDrawerId)
        {

        }

        public void DeassignCashDrawer(int iEmployeeId, int iDrawerId, int iIsPublic)
        {

        }

        public void ReopenCheck(int iEmployeeId, int iQueueId, int iTableId, int iCheckId)
        {

        }

        public void EnterIberScreen(int iTermId, int iScreenId)
        {

        }

        public void ExitIberScreen(int iTermId, int iScreenId)
        {

        }

        public void LogIn(int iEmployeeId, string sName)
        {

        }

        public void IamMaster()
        {

        }

        public void XOpenCheck(int employeeId, int queueId, int tableId, int checkId)
        {

        }

        public void KitchenOrderStatus(string sOrders)
        {

        }

        public void RenameTab(int iTermId, int iCheckId, string stabName)
        {

        }
    }
}
