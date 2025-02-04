using BepInEx;
using UnityEngine;

namespace Checkpoint
{
    [BepInPlugin(Константы.GUID, Константы.Name, Константы.Version)]
    public class Плагин : BaseUnityPlugin
    {
        public static bool ВМоде => NetworkSystem.Instance.InRoom && NetworkSystem.Instance.GameModeString.Contains("MODDED");
        private static bool правый_индекс, левый_индекс, телепорт_использован, Инициализировано;
        private static GameObject чекпоинт;

        void Start() => GorillaTagger.OnPlayerSpawned(Инициализация);

        private void Инициализация()
        {
            чекпоинт = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            чекпоинт.transform.position = new Vector3(-66.8761f, 11.8781f, -82.3425f);
            чекпоинт.transform.localScale = Vector3.one * 0.2f;
            чекпоинт.name = "Чекпоинт";
            Destroy(чекпоинт.GetComponent<SphereCollider>());
            ИзменитьЦвет(Color.green);

            Инициализировано = true;
        }

        void Update()
        {
            if (ВМоде && Инициализировано)
            {
                левый_индекс = ControllerInputPoller.instance.leftControllerIndexFloat > 0.5f;
                правый_индекс = ControllerInputPoller.instance.rightControllerIndexFloat > 0.5f;
                чекпоинт.SetActive(false);
                чекпоинт.SetActive(true);

                if (левый_индекс && !правый_индекс)
                {
                    чекпоинт.transform.SetParent(GorillaTagger.Instance.leftHandTransform);
                    чекпоинт.transform.localPosition = Vector3.zero;
                }
                else if (!левый_индекс)
                {
                    чекпоинт.transform.SetParent(null);
                }

                if (правый_индекс && !левый_индекс && !телепорт_использован)
                {
                    GorillaLocomotion.Player.Instance.TeleportTo(чекпоинт.transform);
                    телепорт_использован = true;
                    ИзменитьЦвет(Color.red);
                }
                else if (!правый_индекс)
                {
                    телепорт_использован = false;
                    ИзменитьЦвет(Color.green);
                }

                чекпоинт.transform.rotation = GorillaLocomotion.Player.Instance.headCollider.transform.rotation;
            }
            else
            {
                чекпоинт.SetActive(false);
            }
        }

        private static void ИзменитьЦвет(Color цвет)
        {
            if (чекпоинт?.GetComponent<Renderer>() is Renderer рендерер)
            {
                рендерер.material.shader = Shader.Find("GorillaTag/UberShader");
                рендерер.material.color = цвет;
            }
        }
    }
}