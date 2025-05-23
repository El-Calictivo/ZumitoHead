using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IRespawnable
{
    UniTask Despawn();
    UniTask Respawn();
}