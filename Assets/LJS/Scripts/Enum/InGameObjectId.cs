public enum InGameObjectId
{

    // 1xxxx
    #region UNIT
    // Civilian units
    // Non Combat unit
    SETTLER = TypeIdBase.UNIT,
    BUILDER,

    // Military units
    // Combat
    // Reacon
    SCOUT,

    // Melee
    WARRIOR, 
    
    // Ranged
    SLINGER, 
    ARCHER, 
    
    // Anti-Cavalry
    SPEARMAN,
    
    // Light Cavalry

    // Heavy Cavalry
    HEAVY_CHARIOT,
    #endregion

    // 2xxxx
    #region FACILITY
    FARM = TypeIdBase.FACILITY,
    MINE,
    #endregion

    // 3xxxx
    #region DISTRICT, BUILDING
    CAMPUS = TypeIdBase.DISTRICT, 
    COMMERCIAL_HUB, 
    INDUSTRIAL_ZONE,
    #endregion
}