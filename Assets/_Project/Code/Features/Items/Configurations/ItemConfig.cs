using UnityEngine;

namespace _Project.Code.Features.Items.Configurations
{
    public abstract class ItemConfig : ScriptableObject
    {
        [SerializeField] private string _id;

        public string Id => _id;

        public abstract Item CreateItem();
    }
}
