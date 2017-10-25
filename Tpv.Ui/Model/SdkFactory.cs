using LasaFOHLib67;
using System;
using System.Runtime.InteropServices;

namespace Tpv.Ui.Model
{

    public class SdkFactory
    {

        // [DO NOT TRANSLATE] (This tells the translation scanner to ignore string literals after this line until further notice)

        private const string LICENSE = "1F1V212T3O5I0Z2:4H0E1J5N475=5H4J2U3T0U3O284I164R4C3R2O1I3:333X3N221M1G3F";
        private const string RAD_API_LICENSE = @"170P0V2U3V5J102?4H2:1G384<5=5H492S3S0U3R284:164R4P4V2K1T5Q5N545A221N1G5@";
        private const string UPI_FOH_LICENSE = @"190T22054V2Z290K4H2<1E5M4I5=5H4<0<4Y0U3R29471Q4R4N4Z2S1J3831525A0Y1O1K3C";
        private const string PERIPHERAL_LICENSE = @"1F0R1V023T2Z0Z0B4H0C1>5O4E5=5H4J094T0U53114@1Q4R4Q4V2O1=3;5P415A271M1M5G";

        // [TRANSLATE] (This tells the translation scanner to resume scanning string literals after this line)

        public static bool MainWindowShown = false;

        #region PInvoke Definitions
        [ComImport]
        [Guid("B196B28F-BAB4-101A-B69C-00AA00341D07")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IClassFactory2
        {
            [return: MarshalAs(UnmanagedType.Interface)]
            object CreateInstance(
                [In, MarshalAs(UnmanagedType.Interface)] object unused,
                [In, MarshalAs(UnmanagedType.LPStruct)] Guid iid);

            void LockServer(Int32 fLock);

            IntPtr GetLicInfo(); // TODO : an enum called LICINFO

            [return: MarshalAs(UnmanagedType.BStr)]
            string RequestLicKey(
                [In, MarshalAs(UnmanagedType.U4)] int reserved);

            [return: MarshalAs(UnmanagedType.Interface)]
            Object CreateInstanceLic(
                [In, MarshalAs(UnmanagedType.Interface)] object pUnkOuter,
                [In, MarshalAs(UnmanagedType.Interface)] object pUnkReserved,
                [In, MarshalAs(UnmanagedType.LPStruct)] Guid iid,
                [In, MarshalAs(UnmanagedType.BStr)] string bstrKey);
        }

        [Flags]
        public enum Clsctx : uint
        {
            ClsctxInprocServer = 0x1,
            ClsctxInprocHandler = 0x2,
            ClsctxLocalServer = 0x4,
            ClsctxInprocServer16 = 0x8,
            ClsctxRemoteServer = 0x10,
            ClsctxInprocHandler16 = 0x20,
            ClsctxReserved1 = 0x40,
            ClsctxReserved2 = 0x80,
            ClsctxReserved3 = 0x100,
            ClsctxReserved4 = 0x200,
            ClsctxNoCodeDownload = 0x400,
            ClsctxReserved5 = 0x800,
            ClsctxNoCustomMarshal = 0x1000,
            ClsctxEnableCodeDownload = 0x2000,
            ClsctxNoFailureLog = 0x4000,
            ClsctxDisableAaa = 0x8000,
            ClsctxEnableAaa = 0x10000,
            ClsctxFromDefaultContext = 0x20000,
            ClsctxInproc = ClsctxInprocServer | ClsctxInprocHandler,
            ClsctxServer = ClsctxInprocServer | ClsctxLocalServer | ClsctxRemoteServer,
            ClsctxAll = ClsctxServer | ClsctxInprocHandler
        }

        [DllImport("ole32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Interface)]
        private static extern object CoGetClassObject(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid rclsid,
            Clsctx dwClsContext,
            IntPtr pServerInfo,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid);

        #endregion

        ///------------------------------------------------------------------------
        /// <summary>
        /// Generic method for instatiating FOH COM interfaces.
        /// </summary>
        /// <typeparam name="TInterface">Iterface type.</typeparam>
        /// <typeparam name="TType">Class that implements interface.</typeparam>
        /// <param name="license">
        /// License to instantiate the requested interface.
        /// </param>
        /// <returns>TInterface</returns>
        ///------------------------------------------------------------------------
        internal static TInterface GetInterfaceInstance<TInterface, TType>(string license)
            where TInterface : class
            where TType : TInterface, new()
        {
            return GetInterfaceInstance<TInterface, TType>(license, null);
        }

        ///------------------------------------------------------------------------
        /// <summary>
        /// Generic method for instatiating FOH COM interfaces.
        /// </summary>
        /// <typeparam name="TInterface">Iterface type.</typeparam>
        /// <typeparam name="TType">Class that implements interface.</typeparam>
        /// <param name="license">
        /// License to instantiate the requested interface.
        /// </param>
        /// <param name="errorText">
        /// Text to display to the user if a failure occurs. If this value is
        /// null, no message will be display to use. Failures are alwyas written to
        /// the log file.
        /// </param>
        /// <returns>TInterface</returns>
        ///------------------------------------------------------------------------
        internal static TInterface GetInterfaceInstance<TInterface, TType>(string license,
                                                                           string errorText)
            where TInterface : class
            where TType : TInterface, new()
        {
            IClassFactory2 icf2 = null;
            TInterface instance = null;



            try
            {
                icf2 = CoGetClassObject(typeof(TType).GUID,
                    Clsctx.ClsctxAll, new IntPtr(),
                    typeof(IClassFactory2).GUID) as IClassFactory2;
            }
            catch (Exception)
            {
                try
                {
                    instance = new TType(); //usu. fails due to CLASS_E_NOTLICENSED 
                }
                catch (InvalidCastException)
                {
                    //anException.Message.Trim();
                }
                catch (Exception)
                {
                    //anExcep.Message.Trim();
                }
            }

            if (icf2 != null)
            {
                instance = icf2.CreateInstanceLic(null, null, typeof(TInterface).GUID,
                                                  license) as TInterface;
            }

            // If we failed to acquire the requested interface and the caller supplied
            // some error text, display the error text to the user now.
            if ((instance == null)
                && (errorText != null)
                && MainWindowShown)
            {
                throw new Exception("Licencia no válida del POS");
            }



            return instance;
        }

        public static IIberFuncs GetIberFuncsInstance()
        {
            return GetInterfaceInstance<IIberFuncs, IberFuncsClass>(LICENSE);
        }

        public static IIberFuncs12 GetIberFuncs12Instance()
        {
            return GetInterfaceInstance<IIberFuncs12, IberFuncsClass>(LICENSE);
        }

        public static IIberFuncs20 GetIberFuncs20Instance()
        {
            return GetInterfaceInstance<IIberFuncs20, IberFuncsClass>(LICENSE);
        }

        public static IIberFuncs17 GetIberFuncs17Instance()
        {
            return GetInterfaceInstance<IIberFuncs17, IberFuncsClass>(LICENSE);
        }

        public static IIberDepot GetIberDepotInstance()
        {
            return GetInterfaceInstance<IIberDepot, IberDepotClass>(LICENSE);

        }

        public static IIberPrinter GetIberPrinterInstance()
        {
            return GetInterfaceInstance<IIberPrinter, IberPrinterClass>(LICENSE);

            // [TRANSLATE] (This tells the translation scanner to resume scanning string literals after this line)

        }

        public static IIberGiftCardActivation GetIberGiftCardInstance()
        {
            return GetInterfaceInstance<IIberGiftCardActivation, IberFuncsClass>(LICENSE);
        }

        public static IARadApi GetIaRadApiInstance()
        {
            return GetInterfaceInstance<IARadApi, CARadApiClass>(RAD_API_LICENSE);
        }

        public static IARadApi2 GetIaRadApi2Instance()
        {
            return GetInterfaceInstance<IARadApi2, CARadApiClass>(RAD_API_LICENSE);
        }

        public static IARadApi3 GetIaRadApi3Instance()
        {
            return GetInterfaceInstance<IARadApi3, CARadApiClass>(RAD_API_LICENSE);
        }

        public static IARadApi4 GetIaRadApi4Instance()
        {
            return GetInterfaceInstance<IARadApi4, CARadApiClass>(RAD_API_LICENSE);
        }

        public static InterceptAlohaPeripherals GetInterceptAlohaPeripherals()
        {
            InterceptAlohaPeripherals intercept = GetInterfaceInstance<InterceptAlohaPeripherals, InterceptAlohaPeripheralsClass>(PERIPHERAL_LICENSE);

            if (intercept == null)
            {
                intercept = GetInterfaceInstance<InterceptAlohaPeripherals, InterceptAlohaPeripheralsClass>(LICENSE);
            }

            return intercept;
        }

        internal static IARadApi5 GetIaRadApi5Instance()
        {
            return GetInterfaceInstance<IARadApi5, CARadApiClass>(RAD_API_LICENSE);
        }

        internal static IARadApi7 GetIaRadApi7Instance()
        {
            return GetInterfaceInstance<IARadApi7, CARadApiClass>(RAD_API_LICENSE);
        }

        internal static IARadApiAddRefundTable GetIaRadApiAddRefundTableInstance()
        {
            return GetInterfaceInstance<IARadApiAddRefundTable, CARadApiClass>(RAD_API_LICENSE);
        }

        public static IARadRit GetIaRadRitInstance()
        {
            return GetInterfaceInstance<IARadRit, CARadApiClass>(RAD_API_LICENSE);
        }

        internal static IARadApiSuspendOrderingOnTable GetIaRadApiSuspendOrderingOnTableInstance()
        {
            return GetInterfaceInstance<IARadApiSuspendOrderingOnTable, CARadApiClass>(RAD_API_LICENSE);
        }

        internal static IUpiFohUtil GetUpiFohUtilInstance()
        {
            return GetInterfaceInstance<IUpiFohUtil, CUpiFohUtilClass>(UPI_FOH_LICENSE);
        }

        internal static IARadIberFuncs21 GetIaRadIberFuncs21Instance()
        {
            return GetInterfaceInstance<IARadIberFuncs21, CARadApiClass>(RAD_API_LICENSE);
        }

        internal static IARadIberFuncs22 GetIaRadIberFuncs22Instance()
        {
            return GetInterfaceInstance<IARadIberFuncs22, CARadApiClass>(RAD_API_LICENSE);
        }

        public static IARadOrderModeCharge GetIaRadOrderModeChargeInstance()
        {
            return GetInterfaceInstance<IARadOrderModeCharge, CARadApiClass>(RAD_API_LICENSE);
        }

        internal static IARadApiClearDanglingOrderLock GetIaRadApiClearDanglingOrderLockInstance()
        {
            return GetInterfaceInstance<IARadApiClearDanglingOrderLock, CARadApiClass>(RAD_API_LICENSE);
        }

        public static IARadIberFuncs23 GetIaRadIberFuncs23Instance()
        {
            return GetInterfaceInstance<IARadIberFuncs23, CARadApiClass>(RAD_API_LICENSE);
        }

        public static CARadApi GetCaRadApi()
        {
            return GetInterfaceInstance<CARadApi, CARadApiClass>(RAD_API_LICENSE);
        }

    }
}
