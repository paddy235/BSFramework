using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace BSFrameWork.Application.AppInterface
{
    public class FaceImportDLL
    {
        private const string dllName = "MPALLibFaceRecFInf";

        [DllImport(dllName, EntryPoint = "FRGetEncryptUUidInfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr FRGetEncryptUUidInfo();

        [DllImport(dllName, EntryPoint = "FRInitAL", CallingConvention = CallingConvention.Cdecl)]
        public static extern int FRInitAL();

        [DllImport(dllName, EntryPoint = "FRUnInitAL", CallingConvention = CallingConvention.Cdecl)]
        public static extern int FRUnInitAL();

        [DllImport(dllName, EntryPoint = "FRInitWithCfg", CallingConvention = CallingConvention.Cdecl)]
        public static extern int FRInitWithCfg(string strCfg, int iVerbose);

        [DllImport(dllName, EntryPoint = "FRSetDetectionPara", CallingConvention = CallingConvention.Cdecl)]
        public static extern int FRSetDetectionPara(int iMinFaceSize, float fMinFaceRatio, int iBestPolicy);

        [DllImport(dllName, EntryPoint = "FRSetDetectionType", CallingConvention = CallingConvention.Cdecl)]
        public static extern int FRSetDetectionType(int iType);

        [DllImport(dllName, EntryPoint = "FRSetLicensePath", CallingConvention = CallingConvention.Cdecl)]
        public static extern int FRSetLicensePath(String licensePath);

        [DllImport(dllName, EntryPoint = "GetFeaturesFromFile", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetFeaturesFromFile([In, Out, MarshalAs(UnmanagedType.LPArray)]byte[] pImageData, int iDateLen, [In, Out, MarshalAs(UnmanagedType.LPArray)]byte[] pFetData);

        [DllImport(dllName, EntryPoint = "GetFeaturesFromFileWithRect", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetFeaturesFromFileWithRect([In, Out, MarshalAs(UnmanagedType.LPArray)]byte[] pImageData, int iDateLen, [In, Out, MarshalAs(UnmanagedType.LPArray)]byte[] pFetData, int x, int y, int width, int height);

        [DllImport(dllName, EntryPoint = "GetFeatureLength", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetFeatureLength();

        [DllImport(dllName, EntryPoint = "GetFeatureSimilarity", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetFeatureSimilarity([In, Out, MarshalAs(UnmanagedType.LPArray)]byte[] jarg1, [In, Out, MarshalAs(UnmanagedType.LPArray)]byte[] jarg2, ref float pfScore);


        [DllImport(dllName, EntryPoint = "GetSimilarity", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetSimilarity([In, Out, MarshalAs(UnmanagedType.LPArray)]byte[] jarg1, int faceFile1, [In, Out, MarshalAs(UnmanagedType.LPArray)]byte[] jarg2, int faceFile2, ref float pfScore);

        [DllImport(dllName, EntryPoint = "FRDetection", CallingConvention = CallingConvention.Cdecl)]
        public static extern int FRDetection([In, Out, MarshalAs(UnmanagedType.LPArray)]byte[] jarg, int faceFile, [In, Out, MarshalAs(UnmanagedType.LPArray)]byte[] pResult, int iResult);

        [DllImport(dllName, EntryPoint = "GetFaceAttribute", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetFaceAttribute([In, Out, MarshalAs(UnmanagedType.LPArray)]byte[] pImageData, int iDateLen, [In, Out, MarshalAs(UnmanagedType.LPArray)]byte[] pResult, int iRetLen, int x, int y, int width, int height);

    }
}
