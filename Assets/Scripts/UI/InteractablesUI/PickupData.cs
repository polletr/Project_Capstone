using System;
using UnityEngine;

[Serializable]
public struct PickupData
{
   [field: SerializeField] public string PickupName { get; set; }
   [field: SerializeField] public Sprite PickupSprite { get; set; }

}