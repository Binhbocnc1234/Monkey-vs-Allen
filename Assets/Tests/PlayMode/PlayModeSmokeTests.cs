using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class PlayModeSmokeTests {
    [Test]
    public void PlayMode_IsRunningInPlayMode() {
        Assert.IsTrue(Application.isPlaying);
    }

    [Test]
    public void GameObject_CanBeCreatedAndDestroyed() {
        GameObject go = new GameObject("PlayModeSmoke_GameObject");

        Assert.IsNotNull(go);
        Assert.AreEqual("PlayModeSmoke_GameObject", go.name);

        Object.DestroyImmediate(go);
    }
}
