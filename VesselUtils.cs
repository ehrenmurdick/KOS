﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kOS
{
    public class VesselUtils
    {
        public static List<Part> GetListOfActivatedEngines(Vessel vessel)
        {
            var retList = new List<Part>();

            foreach (Part part in vessel.Parts)
            {
                foreach (PartModule module in part.Modules)
                {
                    if (module is ModuleEngines)
                    {
                        var engineMod = (ModuleEngines)module;

                        if (engineMod.getIgnitionState)
                        {
                            retList.Add(part);
                        }
                    }
                }
            }

            return retList;
        }

        public static float GetResource(Vessel vessel, string resourceName)
        {
            float total = 0;
            resourceName = resourceName.ToUpper();

            foreach (Part part in vessel.parts)
            {
                foreach (PartResource resource in part.Resources)
                {
                    if (resource.resourceName.ToUpper() == resourceName)
                    {
                        total += (float)resource.amount;
                    }
                }
            }

            return total;
        }

        public static float GetMaxThrust(Vessel vessel)
        {
            var thrust = 0.0;
            ModuleEngines e;

            foreach (Part p in vessel.parts)
            {
                foreach (PartModule pm in p.Modules)
                {
                    if (!pm.isEnabled) continue;
                    if (pm is ModuleEngines)
                    {
                        e = (pm as ModuleEngines);
                        if (!e.EngineIgnited) continue;
                        thrust += e.maxThrust;
                    }
                }
            }

            return (float)thrust;
        }

        public static Vessel TryGetVesselByName(String name, Vessel origin)
        {
            foreach (Vessel v in FlightGlobals.Vessels)
            {
                if (v != origin && v.vesselName.ToUpper() == name.ToUpper())
                {
                    return v;
                }
            }

            return null;
        }

        public static Vessel GetVesselByName(String name, Vessel origin)
        {
            Vessel vessel = TryGetVesselByName(name, origin);

            if (vessel == null)
            {
                throw new kOSException("Vessel '" + name + "' not found");
            }
            else
            {
                return vessel;
            }
        }

        public static void SetTarget(ITargetable val)
        {
            FlightGlobals.fetch.SetVesselTarget(val);
        }

        public static float GetCommRange(Vessel vessel)
        {
            float range = 75000;

            foreach (Part part in vessel.parts)
            {
                if (part.partInfo.name == "longAntenna" && ((ModuleAnimateGeneric)part.Modules["ModuleAnimateGeneric"]).status == "Fixed")
                {
                    range += 75000;
                }
            }

            foreach (Part part in vessel.parts)
            {
                if (part.partInfo.name == "commDish" && ((ModuleAnimateGeneric)part.Modules["ModuleAnimateGeneric"]).status == "Fixed")
                {
                    range *= 10;
                }
            }

            return range;
        }
    }
}
