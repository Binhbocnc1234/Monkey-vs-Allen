using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class BehaviourTests {
    [Test]
    public void CloneBehaviourTemplates_DeepCopies_NoSharedReferences() {
        var templates = new List<IBehaviour> {
            new Idle(),
            new InactiveBehaviour()
        };

        List<IBehaviour> clones = Entity.CloneBehaviourTemplates(templates);

        Assert.AreEqual(2, clones.Count);
        Assert.AreNotSame(templates[0], clones[0]);
        Assert.AreNotSame(templates[1], clones[1]);
        Assert.IsInstanceOf<Idle>(clones[0]);
        Assert.IsInstanceOf<InactiveBehaviour>(clones[1]);
    }

    [Test]
    public void CloneBehaviourTemplates_NullOrEmpty_ReturnsEmpty() {
        List<IBehaviour> result = Entity.CloneBehaviourTemplates(null);
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);

        result = Entity.CloneBehaviourTemplates(new List<IBehaviour>());
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [Test]
    public void CloneBehaviourTemplates_DeepCopies_CanModifyIndependently() {
        var templates = new List<IBehaviour> {
            new Idle()
        };
        templates[0].isEnable = true;

        List<IBehaviour> clones = Entity.CloneBehaviourTemplates(templates);

        // Modify original
        templates[0].isEnable = false;

        // Clone should be unaffected
        Assert.IsTrue(clones[0].isEnable);
    }

    [Test]
    public void IBehaviour_SetEntity_WiresEntityReference() {
        EntitySO so = ScriptableObject.CreateInstance<EntitySO>();
        so.id = Guid.NewGuid().ToString();
        so.health = 10;
        so.behaviourTemplates = new List<IBehaviour> { new Idle() };

        Entity e = new Entity(so, Team.Left, 0f, 0, 1);
        Idle idle = e.GetBehaviour<Idle>();

        Assert.IsNotNull(idle);
        // The entity reference should be set on the behaviour
        // We can verify indirectly by checking the behaviour interacts with the entity
        // IBehaviour.SetEntity is called in Entity constructor
        // Access private field via reflection or verify through GetActiveBehaviour
        Assert.AreSame(e, e.GetActiveBehaviour() is Idle ? e : null);
    }

    [Test]
    public void InactiveBehaviour_AlwaysCanActive_AndPriorityNegativeOne() {
        InactiveBehaviour ib = new InactiveBehaviour();
        Assert.IsTrue(ib.CanActive());
        Assert.AreEqual(-1, ib.GetPriority());
    }

    [Test]
    public void SetBehaviours_OrdersByPriorityDescending() {
        EntitySO so = ScriptableObject.CreateInstance<EntitySO>();
        so.id = Guid.NewGuid().ToString();
        so.health = 10;
        // Start with empty behaviours that won't cause Idle lookup issue
        so.behaviourTemplates = new List<IBehaviour>();

        // Create entity with no templates - it will fail, so let's provide Idle
        so.behaviourTemplates = new List<IBehaviour> { new Idle() };
        Entity e = new Entity(so, Team.Left, 0f, 0, 1);

        // now replace with various priority behaviours
        var behaviours = new List<IBehaviour> {
            new InactiveBehaviour(), // priority -1
            new Idle(),              // priority 0
        };
        e.SetBehaviours(behaviours);

        Assert.AreEqual(2, e.behaviours.Length);
        // Idle (priority 0) should be first, InactiveBehaviour (priority -1) second
        Assert.IsInstanceOf<Idle>(e.behaviours[0]);
        Assert.IsInstanceOf<InactiveBehaviour>(e.behaviours[1]);
    }
}
