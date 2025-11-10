namespace Imported.Samples.Character_Controller._1._3._12.Standard_Characters.Common.Scripts
{
    public struct FixedInputEvent
    {
        private byte _wasEverSet;
        private uint _lastSetTick;

        public void Set(uint tick)
        {
            _lastSetTick = tick;
            _wasEverSet = 1;
        }

        public readonly bool IsSet(uint tick)
        {
            if (_wasEverSet == 1)
            {
                return tick == _lastSetTick;
            }

            return false;
        }
    }
}