using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Logic.Tools
{
    public class OverlayManager
    {
        private static Transform _overlayRoot;
        private static readonly Dictionary<Overlay, Transform> Overlays = new Dictionary<Overlay, Transform>();

        public static void SwapToDefault()
        {
            SwapOverlays(true, true, false);
        }

        public static void SwapOverlays(bool isMiningActive, bool isBuildingActive, bool isFamingActive)
        {
            if(_overlayRoot == null)
                InitOverlays();
            Overlays[Overlay.Mining].gameObject.SetActive(isMiningActive);
            Overlays[Overlay.Building].gameObject.SetActive(isBuildingActive);
            Overlays[Overlay.Farming].gameObject.SetActive(isFamingActive);
        }

        public static Transform GetOverlay(Overlay overlay)
        {
            if (_overlayRoot == null)
                InitOverlays();
            return Overlays[overlay];
        }

        public static void InitOverlays()
        {
            _overlayRoot = new GameObject("Overlays").transform;
            _overlayRoot.parent = GameObject.Find("UI").transform;
            foreach (var overlay in Enum.GetValues(typeof(Overlay)).Cast<Overlay>())
            {
                Overlays[overlay] = new GameObject(overlay.ToString()).transform;
                Overlays[overlay].parent = _overlayRoot;
            }
        }

    }
    public enum Overlay
    {
        Mining,
        Building,
        Farming
    }
}
