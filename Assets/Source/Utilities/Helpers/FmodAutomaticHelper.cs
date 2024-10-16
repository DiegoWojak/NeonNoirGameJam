using System;
using System.Collections;
using UnityEngine;

namespace Assets.Source.Utilities.Helpers
{
    public static class FmodAutomaticHelper
    {
        public static bool CreateSoundEmitirForComputer(ref GameObject computer, FMODUnity.EventReference _event)
        {
            var _c = computer.AddComponent<FMODUnity.StudioEventEmitter>();
            _c.PlayEvent = FMODUnity.EmitterGameEvent.TriggerEnter;
            _c.StopEvent = FMODUnity.EmitterGameEvent.TriggerExit;

            _c.EventReference = _event;

            return true;
        }
    }
}
