using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IIndicator
{
    public Color color {  get; set; }
    //public enum Type {Boss, Spawner }
    //public Type type { get; set; }
    public bool isActive { get; set; }

    void Detect();

}
