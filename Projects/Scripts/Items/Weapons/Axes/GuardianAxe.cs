namespace Server.Items
{
  public class GuardianAxe : OrnateAxe
  {
    [Constructible]
    public GuardianAxe()
    {
      Attributes.BonusHits = 4;
      Attributes.RegenHits = 1;
    }

    public GuardianAxe(Serial serial) : base(serial)
    {
    }

    public override int LabelNumber => 1073545; // guardian axe

    public override void Serialize(IGenericWriter writer)
    {
      base.Serialize(writer);

      writer.WriteEncodedInt(0); // version
    }

    public override void Deserialize(IGenericReader reader)
    {
      base.Deserialize(reader);

      int version = reader.ReadEncodedInt();
    }
  }
}