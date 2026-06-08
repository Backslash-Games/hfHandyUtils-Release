using System.Collections;
using HFHandyUtils.Data.Random;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class RandomSetTests
{
    // DO NOT ADJUST ANY STATIC VALUES
    private static readonly string s_title = "Test Set";
    private static readonly int s_seed = 0;
    private static readonly int s_length = 10;
    private static readonly Vector2Int s_range = new Vector2Int(0, 10);

    private static readonly byte[] e_bytes = new byte[] { 128, 162, 82, 92, 103, 159, 102, 194, 97, 225 };
    private static readonly float e_bytePercentage = ((float)e_bytes[0] / byte.MaxValue);
    private static readonly float e_byteRange = ((s_range.y - s_range.x) * e_bytePercentage) + s_range.x;

    private static readonly ushort[] e_ushorts = new ushort[] { 33060, 41748, 21186, 23778, 26657, 41006, 26412, 50030, 25138, 57847 };
    private static readonly float e_ushortPercentage = ((float)e_ushorts[0] / ushort.MaxValue);
    private static readonly float e_ushortRange = ((s_range.y - s_range.x) * e_ushortPercentage) + s_range.x;

    private static readonly uint[] e_uints = new uint[] { 2166656768, 2736063488, 1388516096, 1558371584, 1747067648, 2687436800, 1731009792, 3278840320, 1647511296, 3791142400 };
    private static readonly float e_uintPercentage = ((float)e_uints[0] / uint.MaxValue);
    private static readonly float e_uintRange = ((s_range.y - s_range.x) * e_uintPercentage) + s_range.x;

    #region General Tests
    /// <summary>
    ///     Used to test local seed creation of sets. Usage of lettering for sorting in Unity UI
    /// </summary>
    [Test]
    public void a_LocalSeedCreation()
    {
        // Arrange
        RandomSet<byte> t_set = new RandomSet<byte>(s_title, s_length);

        // Act
        int e_localSeed = s_seed + s_title.GetHashCode();

        t_set.Initialize();
        GlobalRandom.SetGlobalSeed(s_seed);

        // Assert
        Assert.AreEqual(e_localSeed, t_set.GetLocalSeed());

        // Cleanup
        t_set.Terminate();
    }
    #endregion

    #region Byte Tests
    /// <summary>
    ///     Tests population (& Next()) of byte sets. Usage of lettering for sorting in Unity UI
    /// </summary>
    [Test]
    public void b1_Byte_PopulationNext()
    {
        // Arrange
        RandomSet<byte> t_set = new RandomSet<byte>(s_title, s_length);

        // Act
        t_set.Initialize();
        GlobalRandom.SetGlobalSeed(s_seed);

        // Assert
        Assert.AreEqual(e_bytes, t_set.GetAllValues());

        // Cleanup
        t_set.Terminate();
    }

    /// <summary>
    ///     Tests percentage accuracy of byte sets. Usage of lettering for sorting in Unity UI
    /// </summary>
    [Test]
    public void b2_Byte_NextPercentage()
    {
        // Arrange
        RandomSet<byte> t_set = new RandomSet<byte>(s_title, s_length);

        // Act
        t_set.Initialize();
        GlobalRandom.SetGlobalSeed(s_seed);

        // Assert
        Assert.AreEqual(e_bytePercentage, t_set.Next_Percentage());

        // Cleanup
        t_set.Terminate();
    }

    /// <summary>
    ///     Tests percentage accuracy of byte sets. Usage of lettering for sorting in Unity UI
    /// </summary>
    [Test]
    public void b3_Byte_RangeInt()
    {
        // Arrange
        RandomSet<byte> t_set = new RandomSet<byte>(s_title, s_length);

        // Act
        t_set.Initialize();
        GlobalRandom.SetGlobalSeed(s_seed);

        // Assert
        Assert.AreEqual(Mathf.RoundToInt(e_byteRange), t_set.Range(s_range.x, s_range.y));

        // Cleanup
        t_set.Terminate();
    }

    /// <summary>
    ///     Tests percentage accuracy of byte sets. Usage of lettering for sorting in Unity UI
    /// </summary>
    [Test]
    public void b4_Byte_RangeFloat()
    {
        // Arrange
        RandomSet<byte> t_set = new RandomSet<byte>(s_title, s_length);

        // Act
        t_set.Initialize();
        GlobalRandom.SetGlobalSeed(s_seed);

        // Assert
        Assert.AreEqual(e_byteRange, t_set.Range((float)s_range.x, s_range.y));

        // Cleanup
        t_set.Terminate();
    }
    #endregion
    #region uShort Tests
    /// <summary>
    ///     Tests population (& Next()) of ushort sets. Usage of lettering for sorting in Unity UI
    /// </summary>
    [Test]
    public void c1_UShort_PopulationNext()
    {
        // Arrange
        RandomSet<ushort> t_set = new RandomSet<ushort>(s_title, s_length);

        // Act
        t_set.Initialize();
        GlobalRandom.SetGlobalSeed(s_seed);

        // Assert
        Assert.AreEqual(e_ushorts, t_set.GetAllValues());

        // Cleanup
        t_set.Terminate();
    }

    /// <summary>
    ///     Tests percentage accuracy of ushort sets. Usage of lettering for sorting in Unity UI
    /// </summary>
    [Test]
    public void c2_UShort_NextPercentage()
    {
        // Arrange
        RandomSet<ushort> t_set = new RandomSet<ushort>(s_title, s_length);

        // Act
        t_set.Initialize();
        GlobalRandom.SetGlobalSeed(s_seed);

        // Assert
        Assert.AreEqual(e_ushortPercentage, t_set.Next_Percentage());

        // Cleanup
        t_set.Terminate();
    }

    /// <summary>
    ///     Tests percentage accuracy of ushort sets. Usage of lettering for sorting in Unity UI
    /// </summary>
    [Test]
    public void c3_UShort_RangeInt()
    {
        // Arrange
        RandomSet<ushort> t_set = new RandomSet<ushort>(s_title, s_length);

        // Act
        t_set.Initialize();
        GlobalRandom.SetGlobalSeed(s_seed);

        // Assert
        Assert.AreEqual(Mathf.RoundToInt(e_ushortRange), t_set.Range(s_range.x, s_range.y));

        // Cleanup
        t_set.Terminate();
    }

    /// <summary>
    ///     Tests percentage accuracy of ushort sets. Usage of lettering for sorting in Unity UI
    /// </summary>
    [Test]
    public void c4_UShort_RangeFloat()
    {
        // Arrange
        RandomSet<ushort> t_set = new RandomSet<ushort>(s_title, s_length);

        // Act
        t_set.Initialize();
        GlobalRandom.SetGlobalSeed(s_seed);

        // Assert
        Assert.AreEqual(e_ushortRange, t_set.Range((float)s_range.x, s_range.y));

        // Cleanup
        t_set.Terminate();
    }
    #endregion
    #region Byte Tests
    /// <summary>
    ///     Tests population (& Next()) of uint sets. Usage of lettering for sorting in Unity UI
    /// </summary>
    [Test]
    public void d1_UInt_PopulationNext()
    {
        // Arrange
        RandomSet<uint> t_set = new RandomSet<uint>(s_title, s_length);

        // Act
        t_set.Initialize();
        GlobalRandom.SetGlobalSeed(s_seed);

        // Assert
        Assert.AreEqual(e_uints, t_set.GetAllValues());

        // Cleanup
        t_set.Terminate();
    }

    /// <summary>
    ///     Tests percentage accuracy of uint sets. Usage of lettering for sorting in Unity UI
    /// </summary>
    [Test]
    public void d2_UInt_NextPercentage()
    {
        // Arrange
        RandomSet<uint> t_set = new RandomSet<uint>(s_title, s_length);

        // Act
        t_set.Initialize();
        GlobalRandom.SetGlobalSeed(s_seed);

        // Assert
        Assert.AreEqual(e_uintPercentage, t_set.Next_Percentage());

        // Cleanup
        t_set.Terminate();
    }

    /// <summary>
    ///     Tests percentage accuracy of uint sets. Usage of lettering for sorting in Unity UI
    /// </summary>
    [Test]
    public void d3_UInt_RangeInt()
    {
        // Arrange
        RandomSet<uint> t_set = new RandomSet<uint>(s_title, s_length);

        // Act
        t_set.Initialize();
        GlobalRandom.SetGlobalSeed(s_seed);

        // Assert
        Assert.AreEqual(Mathf.RoundToInt(e_uintRange), t_set.Range(s_range.x, s_range.y));

        // Cleanup
        t_set.Terminate();
    }

    /// <summary>
    ///     Tests percentage accuracy of uint sets. Usage of lettering for sorting in Unity UI
    /// </summary>
    [Test]
    public void d4_UInt_RangeFloat()
    {
        // Arrange
        RandomSet<uint> t_set = new RandomSet<uint>(s_title, s_length);

        // Act
        t_set.Initialize();
        GlobalRandom.SetGlobalSeed(s_seed);

        // Assert
        Assert.AreEqual(e_uintRange, t_set.Range((float)s_range.x, s_range.y));

        // Cleanup
        t_set.Terminate();
    }
    #endregion
}
