using KinematicCharacterController;

using UnityEngine;
using UnityEngine.Playables;

public struct MyMovingPlatformState 
{
    public PhysicsMoverState MoverState;
    public float DirectorTime;
}

public class My3DPlatformMover : MonoBehaviour , IMoverController
{
    public PhysicsMover Mover;

    public PlayableDirector Director;

    private Transform _transform;

    public void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime)
    {
        Vector3 _positionBeforeAnim = _transform.position;
        Quaternion _rotationBeforeAnim = _transform.rotation;
        
        EvaluateAtTime(Time.time);
        
        goalPosition = _transform.position;
        goalRotation = _transform.rotation;

        _transform.position = _positionBeforeAnim;
        _transform.rotation = _rotationBeforeAnim;
    }

    void Start()
    {
        _transform = this.transform; //???
        Mover.MoverController = this;
    }

    public void EvaluateAtTime(double time)
    {
        Director.time = time % Director.duration;
        Director.Evaluate();
    }

}
