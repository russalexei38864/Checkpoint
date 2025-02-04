using BepInEx;
using UnityEngine;

namespace Checkpoint
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Checkpoint : BaseUnityPlugin
    {
        public static bool inRoom => NetworkSystem.Instance.InRoom && NetworkSystem.Instance.GameModeString.Contains("MODDED");
        private static bool right_trigger, left_trigger, debounce_test, CheckpointInitialized;
        private static GameObject checkpoint;

        void Start() => GorillaTagger.OnPlayerSpawned(OnPlayerSpawn);

        private void OnPlayerSpawn()
        {
            checkpoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            checkpoint.transform.position = new Vector3(-66.8761f, 11.8781f, -82.3425f);
            checkpoint.transform.localScale = Vector3.one * 0.2f;
            checkpoint.name = "checkpoint";
            Destroy(checkpoint.GetComponent<SphereCollider>());
            SetCheckpointColor(Color.green);

            CheckpointInitialized = true;
        }

        void Update()
        {
            if (inRoom && CheckpointInitialized)
            {
                left_trigger = ControllerInputPoller.instance.leftControllerIndexFloat > 0.5f;
                right_trigger = ControllerInputPoller.instance.rightControllerIndexFloat > 0.5f;
                checkpoint.SetActive(false);
                checkpoint.SetActive(true);

                if (left_trigger && !right_trigger)
                {
                    checkpoint.transform.SetParent(GorillaTagger.Instance.leftHandTransform);
                    checkpoint.transform.localPosition = Vector3.zero;
                }
                else if (!left_trigger)
                {
                    checkpoint.transform.SetParent(null);
                }

                if (right_trigger && !left_trigger && !debounce_test)
                {
                    GorillaLocomotion.Player.Instance.TeleportTo(checkpoint.transform);
                    debounce_test = true;
                    SetCheckpointColor(Color.red);
                }
                else if (!right_trigger)
                {
                    debounce_test = false;
                    SetCheckpointColor(Color.green);
                }

                checkpoint.transform.rotation = GorillaLocomotion.Player.Instance.headCollider.transform.rotation;
            }
            else
            {
                checkpoint.SetActive(false);
            }
        }

        private static void SetCheckpointColor(Color цвет)
        {
            if (checkpoint?.GetComponent<Renderer>() is Renderer рендерер)
            {
                рендерер.material.shader = Shader.Find("GorillaTag/UberShader");
                рендерер.material.color = цвет;
            }
        }
    }
}