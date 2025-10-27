using System.Collections.Generic;

namespace _Project.Code.Features.Character.MB.EffectsSystem
{
    public abstract class BaseEffect: ICharacterEffect
    {
        protected List<ICharacterEffectModifier> Modifiers = new List<ICharacterEffectModifier>();
        
        public EffectHandle AddModifier(ICharacterEffectModifier modifier)
        {
            Modifiers.Add(modifier);
            return new EffectHandle(modifier, RemoveModifier);
        }
        
        public void Tick(float dt)
        {
            foreach (var modifier in Modifiers)
            {
                modifier.Evaluate(dt);
            }
            
            Modifiers.RemoveAll(m => m.IsExpired);
            
            ComputeResult();
        }
        
        protected abstract void ComputeResult();
        
        private void RemoveModifier(ICharacterEffectModifier modifier)
        {
            Modifiers.Remove(modifier);
        }
    }
}
