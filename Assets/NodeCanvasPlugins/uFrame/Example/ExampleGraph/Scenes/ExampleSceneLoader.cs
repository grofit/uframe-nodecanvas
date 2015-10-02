using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using uFrame.IOC;
using uFrame.Kernel;
using uFrame.MVVM;
using uFrame.Serialization;
using UnityEngine;


public class ExampleSceneLoader : ExampleSceneLoaderBase {
    
    protected override IEnumerator LoadScene(ExampleScene scene, Action<float, string> progressDelegate) {
        yield break;
    }
    
    protected override IEnumerator UnloadScene(ExampleScene scene, Action<float, string> progressDelegate) {
        yield break;
    }
}
