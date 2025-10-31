namespace _Project.Code.Features.Character.MB.EffectsSystem.EffectValues
{
    public class ConstantEffectValue<T> : ICharacterEffectValue<T>
    {
        private T _value;
        
        public ConstantEffectValue(T value)
        {
            _value = value;
        }
        
        public void Tick(float dt) { }

        public T GetValue()
        {
            return _value;
        }
    }
}