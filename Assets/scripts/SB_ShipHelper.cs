using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SB_ShipHelper
{
    public static List<Ship> ObjectToShips(List<object> ships_as_objects)
    {
        List<Ship> ship_list = new List<Ship>();
        foreach (IDictionary<string, object> ship in ships_as_objects)
        {
            ship_list.Add(shipFromObject(ship));
        }
        return ship_list;
    }

    public static Ship shipFromObject(IDictionary<string, object> ship)
    {
        Ship tmpShip = new Ship();
        System.Type shipType = typeof(Ship);
        foreach (var item in ship)
        {
            var field = shipType.GetField(item.Key);
            if (field != null && item.Value != null)
            {
                if (item.Value.GetType() == typeof(System.Double))
                {
                    field.SetValue(tmpShip, System.Convert.ToSingle(item.Value));
                }
                else if (item.Key == "position")
                {
                    Position position = new Position();
                    position.x = 0;
                    position.y = 0;
                    field.SetValue(tmpShip, position);
                }
                else
                {
                    field.SetValue(tmpShip, item.Value);
                }

            }
        }
        return tmpShip;
    }
}
