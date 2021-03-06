using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Settings
{
    public class ChangeTownOptions : OperationSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        public override string GetTitlePrefix()
        {
            return "GlobalOptions";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
 	        if (!base.Allow(parameters)) return false;

            Sim sim = parameters.mTarget as Sim;
            if (sim != null)
            {
                if (sim.IsActiveSim)
                {
                    return true;
                }
            }

            return Common.IsRootMenuObject(parameters.mTarget);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            StoryProgression.Main.Options.ShowOptions(StoryProgression.Main, Common.Localize("GlobalOptions:MenuName"));
            return OptionResult.SuccessRetain;
        }
    }
}

