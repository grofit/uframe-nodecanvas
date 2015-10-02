using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using uFrame.Kernel;
using uFrame.IOC;
using uFrame.MVVM;
using uFrame.Serialization;


public class EntityController : EntityControllerBase {
    
    public override void InitializeEntity(EntityViewModel viewModel) {
        base.InitializeEntity(viewModel);
        // This is called when a EntityViewModel is created
    }
}
