using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TechnologyId
{
    Pottery = TypeIdBase.TECHNOLOGY + EraId.Ancient,
    AnimalHusbandry,
    Mining,

    Irrigation,
    Writing,
    Archery,

    Masonry,
    BronzeWorking,
    Wheel,

    Currency = TypeIdBase.TECHNOLOGY + EraId.Classical,
    HorsebackRiding,
    IronWorking,

    Mathematics,
    Construction,
    Engineering,

    MilitaryTactics = TypeIdBase.TECHNOLOGY + EraId.Medieval,
    Apprenticeship,
    Machinery,

    Education,
    Stirrups,
    MilitaryEngineering,
    Castles,

    MassProduction = TypeIdBase.TECHNOLOGY + EraId.Renaissance,
    Baking,
    Printing,
    Gunpowder,

    SiegeTactics,
    MetalCasting,
    Astronomy

}
