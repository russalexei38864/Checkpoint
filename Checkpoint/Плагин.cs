using BepInEx;
using UnityEngine;

namespace Checkpoint
{
    [BepInPlugin(Константы.GUID, Константы.Name, Константы.Version)]
    public class Плагин : BaseUnityPlugin
    {
        public static bool ВМоде => NetworkSystem.Instance.InRoom && NetworkSystem.Instance.GameModeString.Contains("MODDED");
        private static bool левый_индекс, правый_индекс, телепорт_использован;
        private static GameObject чекпоинт;
        private static bool инициализировано, включено;

        void Start() => GorillaTagger.OnPlayerSpawned(Инициализация);

        void OnEnable()
        {
            включено = true;
            чекпоинт?.SetActive(true);
        }

        void OnDisable()
        {
            включено = false;
            чекпоинт?.SetActive(false);
        }

        void Инициализация()
        {
            if (инициализировано) return;

            чекпоинт = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            чекпоинт.transform.SetPositionAndRotation(new Vector3(-66.8761f, 11.8781f, -82.3425f), Quaternion.identity);
            чекпоинт.transform.localScale = Vector3.one * 0.2f;
            чекпоинт.name = "Чекпоинт";
            Destroy(чекпоинт.GetComponent<SphereCollider>());

            ОбновитьЦвет(Color.green);
            чекпоинт.SetActive(false);
            инициализировано = true;
        }

        void Update()
        {
            if (!ВМоде || !инициализировано || !включено)
            {
                чекпоинт?.SetActive(false);
                return;
            }

            ОбновитьСостояние();
        }

        private void ОбновитьСостояние()
        {
            левый_индекс = ControllerInputPoller.instance.leftControllerIndexFloat > 0.5f;
            правый_индекс = ControllerInputPoller.instance.rightControllerIndexFloat > 0.5f;
            чекпоинт.SetActive(true);

            if (левый_индекс && !правый_индекс)
            {
                чекпоинт.transform.SetParent(GorillaTagger.Instance.leftHandTransform, false);
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
            }
            else if (!правый_индекс)
            {
                телепорт_использован = false;
            }

            ОбновитьЦвет(правый_индекс || левый_индекс ? Color.red : Color.green);

            чекпоинт.transform.rotation = GorillaLocomotion.Player.Instance.headCollider.transform.rotation;
        }

        private static void ОбновитьЦвет(Color цвет)
        {
            if (чекпоинт?.TryGetComponent(out Renderer рендерер) == true)
            {
                рендерер.material.shader = Shader.Find("GorillaTag/UberShader");
                рендерер.material.color = цвет;
            }
        }
    }
}
