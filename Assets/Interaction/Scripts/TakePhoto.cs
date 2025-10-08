using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using Valve.VR.InteractionSystem;

//using MedicalTraining.Utils;
using System.Collections.Generic;
using MedicalTraining.Configuration;
using MedicalTraining.Logger;

public class TakePhoto : MonoBehaviour
{

    public Camera cam, bodyRegionCam;
    public AudioSource audio;
    public Valve.VR.SteamVR_Action_Boolean actionObject;

    [SerializeField]
    private Material bigScreenMaterial;
    [SerializeField]
    private GameObject screen;
    [SerializeField]
    private Valve.VR.InteractionSystem.Interactable interactable;

    private ConfigurationContainer configuration;
    //private SimulationLogger log;
    private RenderTexture _tex;
    private int resultRenderTexWidth;
    private int resultRenderTexHeight;
    private GraphicsFormat resultRenderGraphicsFormat;
    private bool canSave;
    private int _photosTaken = 0;
    private bool _initialized = false;

    public bool PhotoLimitEnabled { get; private set; }
    
    public int PhotoLimit { get; private set; }

    public delegate void SavePhotoAvailableEvent();
    public event SavePhotoAvailableEvent SavePhotoAvailable;

    public delegate void SavePhotoUnavailableEvent();
    public event SavePhotoUnavailableEvent SavePhotoUnavailable;
    
    public delegate void PhotoCountUpdatedEvent();
    public event PhotoCountUpdatedEvent PhotoCountUpdated;
    
    public void Initialize()
    {
        if (_initialized) return;
        _initialized = true;
        
        configuration = ConfigurationContainer.Instance;
        //log = FindObjectOfType<SimulationLogger>();
        cam.enabled = false;
        bodyRegionCam.enabled = false;
        canSave = false;

        // Hide RenderTexture to avoid wrong content due to shared memory
        bigScreenMaterial.color = Color.black;

        // Init result RenderTexture for photo saving
        resultRenderTexWidth = cam.targetTexture.width + bodyRegionCam.targetTexture.width;
        resultRenderTexHeight = Mathf.Max(cam.targetTexture.height, bodyRegionCam.targetTexture.height);
        resultRenderGraphicsFormat = cam.targetTexture.graphicsFormat;
        _tex = new RenderTexture(resultRenderTexWidth, resultRenderTexHeight, cam.targetTexture.depth, resultRenderGraphicsFormat);
        
        string maxPicturesConf = configuration.GetScenarioSpecificSetting("maxPicturesDermatoscope");
        Debug.Log(maxPicturesConf);

        if (maxPicturesConf == null || !int.TryParse(maxPicturesConf, out _))
        {
            Debug.LogWarning("Scenario specific configuration not set or not of type Scenario_SkinCancerScreening. MaxPictures set to 999.");
            PhotoLimit = 999;
            PhotoLimitEnabled = false;
        }

        else
        {
            PhotoLimit = int.Parse(maxPicturesConf);
            PhotoLimitEnabled = true;
        }
        
        Debug.Log($"{PhotoLimit} photo limit, enabled? {PhotoLimitEnabled}");
        PhotoCountUpdated?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        if (interactable.attachedToHand)
        {
            Hand hand = interactable.attachedToHand;
            if (actionObject.GetStateDown(hand.handType))
            {
                if (_photosTaken < PhotoLimit)
                {
                    canSave = true;
                    SavePhotoAvailable?.Invoke();
                }

                cam.Render();
                // Show RenderTexture on screen
                if(screen != null)
                {
                    screen.SetActive(true);
                }
                if (bigScreenMaterial.color != Color.white)
                {
                    bigScreenMaterial.color = Color.white;
                }
                bodyRegionCam.Render();

                // Play sound when capturing image
                audio.Play();
            }
        }
    }

    public void SetScreen(GameObject screen)
    {
        this.screen = screen;
    }

    public int GetPhotosTaken() => _photosTaken;

    public bool SavePhoto(){
        if(!canSave) {
            return false;
        }

        _photosTaken++;
        PhotoCountUpdated?.Invoke();
        canSave = false;
        SavePhotoUnavailable?.Invoke();

        SimulationLogger log = ConfigurationContainer.Instance.GetLogger();

        if (log != null){
            Graphics.CopyTexture(
                src: cam.targetTexture,
                srcElement: 0,
                srcMip: 0,
                srcX: 0,
                srcY: 0,
                srcWidth: cam.targetTexture.width,
                srcHeight: cam.targetTexture.height,
                dst: _tex,
                dstElement: 0,
                dstMip: 0,
                dstX: 0,
                dstY: 0);

            Graphics.CopyTexture(
                src: bodyRegionCam.targetTexture,
                srcElement: 0,
                srcMip: 0,
                srcX: 0,
                srcY: 0,
                srcWidth: bodyRegionCam.targetTexture.width,
                srcHeight: bodyRegionCam.targetTexture.height,
                dst: _tex,
                dstElement: 0,
                dstMip: 0,
                dstX: cam.targetTexture.width,
                dstY: 0);
            
            // Use static coroutine to keep it running even when the GameObject this MonoBehavior is attached to gets disabled
            StaticCoroutine.Start(ReadBackAndSavePhoto(log.GetFilePaths("Photo", "png", readableTimestamp: true)));
            return true;
        }
        return false;
    }

    private IEnumerator ReadBackAndSavePhoto(List<string> savePaths)
    {        
        NativeArray<byte> buffer = new NativeArray<byte>(_tex.width * _tex.height * 4, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

        var request = AsyncGPUReadback.RequestIntoNativeArray(ref buffer, _tex, 0);

        while (!request.done)
        {
            yield return null;
        }

        if (request.hasError)
        {
            // in case of an error dispose native array and stop
            buffer.Dispose();
            Debug.Log("Error while AsyncGPUReadback");
            yield break;
        }

        savePhotoToDisk(buffer, savePaths);
    }

    private async void savePhotoToDisk(NativeArray<byte> buffer, List<string> savePaths)
    {
        using var encoded = await Task.Run(() => {
            return ImageConversion.EncodeNativeArrayToPNG(
            buffer,
            resultRenderGraphicsFormat,
            (uint) resultRenderTexWidth,
            (uint) resultRenderTexHeight
            );
        });

        foreach (string savePath in savePaths)
            await File.WriteAllBytesAsync(savePath, encoded.ToArray());
        buffer.Dispose();
    }

    private void OnDestroy()
    {
        Destroy(_tex);
    }
}
