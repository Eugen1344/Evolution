#if PLAYMAKER

using HutongGames.PlayMaker;

namespace SensorToolkit.PlayMaker 
{
    [ActionCategory("Sensors")]
    [Tooltip("Sets the ignore list array on a sensor")]
    public class SensorSetIgnoreList : SensorToolkitComponentAction<Sensor> 
    {
        [RequiredField]
        [CheckForComponent(typeof(Sensor))]
        [Tooltip("The game object owning the Sensor.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The array of GameObjects to ignore")]
        [ArrayEditor(VariableType.GameObject)]
        public FsmArray ignoreList;

        [Tooltip("Sets the ignore list each frame.")]
        public bool everyFrame;

        public override void Reset() 
        {
            gameObject = null;
            ignoreList = null;
            everyFrame = false;
        }

        public override void OnEnter() 
        {
            doCheck();

            if (!everyFrame) 
            {
                Finish();
            }
        }

        public override void OnUpdate() 
        {
            doCheck();
        }

        void doCheck() 
        {
            if (!UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject))) return;

            if (!ignoreList.IsNone) 
            {
                var length = ignoreList.Length;
                sensor.IgnoreList.Clear();
                for (var i = 0; i < length; i++) 
                {
                    sensor.IgnoreList.Add(ignoreList.Get(i) as UnityEngine.GameObject);
                }
            }
        }
    }
}

#endif