﻿using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.Sims;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.MapTags;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    public abstract class SelectionOptionBaseList<TOption> : CommonOptionList<TOption>, SimSelection.ICriteria, ITestableOption
        where TOption : CommonOptionItem, IPersistence, ITestableOption
    {
        [Persistable(false)]
        bool mEnabled = false;

        List<TOption> mOptions = null;
        bool mMatchAll = false;
        bool mRandomCriteria = false;
        bool mRandomValue = false;

        public SelectionOptionBaseList()
        { }
        public SelectionOptionBaseList(List<TOption> options)
        {
            mOptions = options;
        }

        public string OptionName
        {
            get
            {
                IEnumerable<TOption> options = GetOptions();
                if (options == null) return string.Empty;

                string result = string.Empty;

                foreach (TOption option in options)
                {
                    result += option.Name + Common.NewLine;
                }

                return result;
            }
        }

        public string OptionValue
        {
            get { return string.Empty; }
        }

        public bool Enabled
        {
            get
            {
                return mEnabled;
            }
            set
            {
                mEnabled = value;
            }
        }

        public bool CanBeRandomCriteria
        {
            get
            {
                return mRandomCriteria;
            }
            set
            {
                mRandomCriteria = value;
            }
        }

        public bool CanHaveRandomValue
        {
            get
            {
                return mRandomValue;
            }
            set
            {
                mRandomValue = value;
            }
        }

        public bool CanBeRandomValue
        {
            get { return false; }
            set { }
        }

        public int OptionHitValue
        {
            get { return 0; }
            set { }
        }

        public int OptionMissValue
        {
            get { return 0; }
            set { }
        }

        public abstract string GetTitlePrefix();

        public override string Name
        {
            get
            {
                return Common.Localize(GetTitlePrefix() + ":MenuName");
            }
        }

        public virtual bool IsSpecial
        {
            get { return false; }
        }

        public virtual bool IsRandom
        {
            get { return false; }
        }

        public override string DisplayValue
        {
            get { return null; }
        }

        public override void Reset()
        {
            mEnabled = true;
            mOptions = null;
            mMatchAll = false;

            base.Reset();
        }

        public void Export(Persistence.Lookup settings)
        {
            settings.Add("Option", mOptions);
        }

        public void Import(Persistence.Lookup settings)
        {
            mOptions = settings.GetList<TOption>("Option");
        }

        public string PersistencePrefix
        {
            get { return null; }
        }

        public override List<TOption> GetOptions()
        {
            return mOptions;
        }

        public List<ICommonOptionItem> GetOptions(IMiniSimDescription actor, IEnumerable<SimSelection.ICriteria> criteria, List<IMiniSimDescription> sims)
        {
            if (mOptions == null)
            {
                Update(actor, criteria, sims, false, true, false);
            }

            List<ICommonOptionItem> mOpts = new List<ICommonOptionItem>();

            if (mOptions == null) return mOpts;

            foreach (TOption opt in mOptions)
            {
                mOpts.Add(opt as ICommonOptionItem);
            }

            return mOpts;
        }

        public void SetOptions(List<ICommonOptionItem> options)
        {
            if (mOptions != null)
            {
                mOptions.Clear();
            }
            else
            {
                mOptions = new List<TOption>();
            }

            foreach (ICommonOptionItem opt in options)
            {
                TOption test = opt as TOption;
                if (opt == null)
                {
                    Common.Notify("Opt null");
                }
                mOptions.Add(opt as TOption);
            }
        }

        protected virtual ObjectPickerDialogEx.CommonHeaderInfo<TOption> Auxillary
        {
            get { return null; }
        }

        public abstract void GetOptions(List<IMiniSimDescription> actors, List<IMiniSimDescription> allSims, List<TOption> items);

        public virtual bool AllowCriteria()
        {
            return true;
        }

        public int GetScoreValue(IMiniSimDescription me, IMiniSimDescription actor, bool satisfies, int divisior)
        {
            return 0;
        }

        public bool Test(IMiniSimDescription me, bool fullFamily, IMiniSimDescription actor, bool testRandom)
        {
            return Test(me, fullFamily, actor);
        }

        public bool Test(IMiniSimDescription me, bool fullFamily, IMiniSimDescription actor)
        {
            if (fullFamily)
            {
                SimDescription trueSim = actor as SimDescription;
                if ((trueSim != null) && (trueSim.Household != null) && (!SimTypes.IsSpecial(trueSim)))
                {
                    foreach (SimDescription member in CommonSpace.Helpers.Households.All(trueSim.Household))
                    {
                        if (me is SimDescription)
                        {
                            if (Allow(me as SimDescription, member))
                            {
                                return true;
                            }
                        }
                        else if (me is MiniSimDescription)
                        {
                            if (Allow(me as MiniSimDescription, member))
                            {
                                return true;
                            }
                        }
                    }

                    return false;
                }
            }

            if (me is SimDescription)
            {
                return Allow(me as SimDescription, actor);
            }
            else if (me is MiniSimDescription)
            {
                return Allow(me as MiniSimDescription, actor);
            }
            return false;
        }

        protected virtual bool MatchAll
        {
            get { return mMatchAll; }
        }

        protected virtual bool Allow(SimDescription me, IMiniSimDescription actor)
        {
            IEnumerable<TOption> options = GetOptions();
            if (options == null) return true;

            if (MatchAll)
            {
                foreach (TOption option in options)
                {
                    if (!option.Test(me, false, actor)) return false;
                }

                return true;
            }
            else
            {
                foreach (TOption option in options)
                {
                    if (option.Test(me, false, actor)) return true;
                }

                return false;
            }
        }
        protected virtual bool Allow(MiniSimDescription me, IMiniSimDescription actor)
        {
            IEnumerable<TOption> options = GetOptions();
            if (options == null) return true;

            if (MatchAll)
            {
                foreach (TOption option in options)
                {
                    if (!option.Test(me, false, actor)) return false;
                }

                return true;
            }
            else
            {
                foreach (TOption option in options)
                {
                    if (option.Test(me, false, actor)) return true;
                }

                return false;
            }
        }

        public SimSelection.UpdateResult Update(IMiniSimDescription actor, IEnumerable<SimSelection.ICriteria> criteria, List<IMiniSimDescription> allSims, bool secondStage)
        {
            return Update(actor, criteria, allSims, secondStage, false, false);
        }

        public SimSelection.UpdateResult Update(IMiniSimDescription actor, IEnumerable<SimSelection.ICriteria> criteria, List<IMiniSimDescription> allSims, bool secondStage, bool silent, bool promptForMatchAll)
        {
            if (secondStage) return SimSelection.UpdateResult.Success;

            bool fullFamily = false;
            foreach (SimSelection.ICriteria crit in criteria)
            {
                if (crit is FullFamily)
                {
                    fullFamily = true;
                    break;
                }
            }

            if ((mOptions == null) || (mOptions.Count == 0))
            {
                List<IMiniSimDescription> actors = new List<IMiniSimDescription>();

                if (fullFamily)
                {
                    SimDescription trueActor = actor as SimDescription;
                    if ((trueActor != null) && (trueActor.Household != null) && (!SimTypes.IsSpecial(trueActor)))
                    {
                        foreach (SimDescription member in CommonSpace.Helpers.Households.All(trueActor.Household))
                        {
                            actors.Add(member);
                        }
                    }
                }
                else
                {
                    actors.Add(actor);
                }

                List<TOption> allOptions = new List<TOption>();
                GetOptions(actors, allSims, allOptions);

                if (allOptions.Count == 0) return SimSelection.UpdateResult.Failure;

                if (allOptions.Count == 1 || silent)
                {
                    mOptions = allOptions;
                }
                else
                {
                    mOptions = new List<TOption>(new CommonSelection<TOption>(Name, allOptions, Auxillary).SelectMultiple());
                }
            }

            if ((mOptions != null) && (mOptions.Count > 0))
            {
                for (int i = allSims.Count - 1; i >= 0; i--)
                {
                    if (!Test(allSims[i], fullFamily, actor))
                    {
                        allSims.RemoveAt(i);
                    }
                }

                if (promptForMatchAll)
                {
                    if (TwoButtonDialog.Show(MasterController.Localize("CriteriaMatchAll:Prompt"), MasterController.Localize("CriteriaMatchAll:Yes"), MasterController.Localize("CriteriaMatchAll:No")))
                    {
                        mMatchAll = true;
                    }
                }

                return SimSelection.UpdateResult.Success;
            }
            else
            {
                return SimSelection.UpdateResult.Failure;
            }
        }
    }
}
