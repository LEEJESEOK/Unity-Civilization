using System.Runtime.Serialization;
using System;
/// <summary>
/// Interface that needs to be implemented by any object that gets affected by the Field of View of the player.
/// </summary>
/// 

interface IHideable/* : ISerializable*/
{
    void OnFOVEnterHide();
    void OnFOVLeaveShow();
    void OnFOVTransparency();
}
