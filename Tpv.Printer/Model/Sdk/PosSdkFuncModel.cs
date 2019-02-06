using LasaFOHLib;
using LoyaltyIntegrationLib.Service.Sdk;
using System;
using Tpv.Printer.Model.Shared;

namespace Tpv.Printer.Model.Sdk
{
    public class PosSdkFuncModel
    {
        public IberDepot Depot { get; }
        public IIberFuncs20 Funcs { get; private set; }
        public IberObject LocalState { get; private set; }
        public int TableId { get; set; }
        public int CheckId { get; set; }
        //public PosMasterModel PosMaster { get; set; }

        public PosSdkFuncModel()
        {
            Depot = new IberDepotClass();
        }

        public void InitFunction()
        {

#if DEBUG
            Funcs = new IberFuncs();
#else
            Funcs = SdkFactory.GetIberFuncsInstance() as IIberFuncs20;
#endif

            if (Funcs == null)
                throw new Exception("Invalid license or incorrect POS version");
        }

        public void InitLocalState()
        {
            LocalState = Depot.GetEnum(Constants.InternalPos.INTERNAL_LOCALSTATE).First();
        }

        public void RefreshFuncs()
        {
            Funcs = new IberFuncs();
        }

        public bool IsTableService()
        {
            return Funcs.IsTableService();
        }
    }
}
