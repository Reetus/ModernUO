using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Misc
{
  public class EquipLastWeapon
  {
    public static void Initialize()
    {
      PacketHandlers.RegisterEncoded(0x1E, true, OnEquipLastWeapon);
    }

    private static bool TryDequip(Mobile m, Item weapon)
    {
      if (!weapon.Movable)
      {
        return false;
      }

      if (m.Backpack == null || !m.Backpack.CheckHold(m, weapon, false))
      {
        m.SendLocalizedMessage(1063110); // Your backpack cannot hold the weapon in your hand.
        return false;
      }

      m.Backpack.DropItem(weapon);
      return true;
    }

    private static Item GetWeapon(Mobile m)
    {
      Item weapon = m.FindItemOnLayer(Layer.OneHanded) ?? m.FindItemOnLayer(Layer.TwoHanded);

      if (weapon is BaseWeapon || weapon is Spellbook)
      {
        return weapon;
      }

      return null;
    }

    public static void OnEquipLastWeapon(NetState state, IEntity e, EncodedReader reader)
    {
      if (!(state.Mobile is PlayerMobile pm) || !pm.CheckAlive())
      {
        return;
      }

      if (pm.AccessLevel < AccessLevel.GameMaster || Core.TickCount - pm.NextActionTime < 0)
      {
        pm.SendActionMessage();
        return;
      }

      Item lastWeapon = pm.LastWeapon;

      if (lastWeapon == null || lastWeapon.Deleted)
      {
        lastWeapon = GetWeapon(pm);

        if (lastWeapon == null)
        {
          return;
        }
      }

      if (lastWeapon.Parent == pm)
      {
        TryDequip(pm, lastWeapon);
      }
      else if (!lastWeapon.IsChildOf(pm.Backpack))
      {
        pm.SendLocalizedMessage(1063109); // Your last weapon must be in your backpack to be able to switch it quickly.
        pm.LastWeapon = null;
        return;
      }
      else
      {
        Item currentWeapon = GetWeapon(pm);

        if (currentWeapon != null)
        {
          if (!TryDequip(pm, currentWeapon))
          {
            return;
          }

          pm.SendLocalizedMessage(!pm.EquipItem(lastWeapon) ? 1063113 /* You put your weapon into your backpack, but cannot pick up your last weapon! */ : 1063111 /* You put your weapon into your backpack and pick up your last weapon. */);
        }
        else
        {
          pm.SendLocalizedMessage(!pm.EquipItem(lastWeapon) ? 1063114 /* You cannot pick up your last weapon! */ : 1063112 /* You pick up your last weapon. */);
        }
      }

      pm.NextActionTime = Core.TickCount + Mobile.ActionDelay;
    }
  }
}
