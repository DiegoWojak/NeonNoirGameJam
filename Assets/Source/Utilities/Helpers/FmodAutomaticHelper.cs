using FMODUnity;
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
            _c.PlayEvent = EmitterGameEvent.TriggerEnter;
            //_c.StopEvent = EmitterGameEvent.TriggerExit;

            _c.EventReference = _event;

            return true;
        }

        public static bool CreateSoundEmitirForDoor(ref GameObject Door, 
            EventReference _doorOpenevent, EventReference _doorClosedEvent,
            EmitterGameEvent _forOpen, EmitterGameEvent _forLeave)
        {            
            var InstanceA = Door.AddComponent<StudioEventEmitter>();
            var InstanceB = Door.AddComponent<StudioEventEmitter>();

            InstanceA.PlayEvent = _forOpen;
            InstanceA.EventReference = _doorOpenevent;
            InstanceB.PlayEvent = _forLeave;
            InstanceB.EventReference = _doorClosedEvent;

            return true;
        }
    }
}
