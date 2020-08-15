using System.Collections;
using MelonLoader;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace NoPostProcessing
{
    public class NoPostProcessing : MelonMod
    {
        #region Data Types
            public static bool DisablePostProcessing = true;

            public static bool WorldWasChanged = false;
        #endregion

        public override void OnApplicationStart()
        {
            //Register Config Category And Toggle Boolean If Not Present
            MelonPrefs.RegisterCategory("NoPostProcessing", "NoPostProcessing");
            MelonPrefs.RegisterBool("NoPostProcessing", "DisablePostProcessing", true, "Disable Post Processing");

            //Update Local Boolean: DisablePostProcessing To The State Of The Boolean In Config So We Don't Need To Keep Pulling It From Config, Which Would Cause Unnecessary Disk Reads
            DisablePostProcessing = MelonPrefs.GetBool("NoPostProcessing", "DisablePostProcessing");
        }

        public override void OnModSettingsApplied()
        {
            //Assume This Mod's Config Boolean Was Modified, Even If It Was Not & Act On That Assumption

            //Update Local Boolean: DisablePostProcessing To The State Of The Boolean In Config So We Don't Need To Keep Pulling It From Config, Which Would Cause Unnecessary Disk Reads
            DisablePostProcessing = MelonPrefs.GetBool("NoPostProcessing", "DisablePostProcessing");

            //Enumerate All Cameras
            foreach (Camera cam in Camera.allCameras)
            {
                //If The Camera Has Post Processing Applied To It
                if (cam.GetComponent<PostProcessLayer>() != null)
                {
                    //If The Camera's Post Processing Layer Has Not Been Toggled Previously
                    if (!DisablePostProcessing != cam.GetComponent<PostProcessLayer>().enabled)
                    {
                        //Variably Log If The Post Processing Layer Was Removed Or Added
                        MelonLogger.Log(DisablePostProcessing ? "Removed Post Processing From World!" : "Re-Added Post Processing From World!");

                        //Set The Post Processing Layer On The Camera To The Opposite State Of DisablePostProcessing
                        cam.GetComponent<PostProcessLayer>().enabled = !DisablePostProcessing;
                    }
                }
            }
        }

        public override void OnLevelWasInitialized(int level)
        {
            //If World Was Initialized (Not Login/Loading)
            if (level == -1)
            {
                //Define Boolean Stating The World Was Changed
                WorldWasChanged = true;

                //Define Delay Offset To Use In OnUpdate()
                MelonCoroutines.Start(DelayedAction());
            }
        }

        IEnumerator DelayedAction()
        {
            yield return new WaitForSeconds(5);

            //Do Not Run Any Code Below Unless The Delay Has Passed & The World Has Changed
            if (WorldWasChanged)
            {
                //Define WorldWasChanged As False Until Next World Change
                WorldWasChanged = false;

                //Enumerate All Cameras
                foreach (Camera cam in Camera.allCameras)
                {
                    //If The Camera Has Post Processing Applied To It
                    if (cam.GetComponent<PostProcessLayer>() != null)
                    {
                        //If The Camera's Post Processing Layer Has Not Been Toggled Previously
                        if (!DisablePostProcessing != cam.GetComponent<PostProcessLayer>().enabled)
                        {
                            //Variably Log If The Post Processing Layer Was Removed Or Added
                            MelonLogger.Log(DisablePostProcessing ? "Removed Post Processing From World!" : "Re-Added Post Processing From World!");

                            //Set The Post Processing Layer On The Camera To The Opposite State Of DisablePostProcessing
                            cam.GetComponent<PostProcessLayer>().enabled = !DisablePostProcessing;
                        }
                    }
                }
            }
        }
    }
}
