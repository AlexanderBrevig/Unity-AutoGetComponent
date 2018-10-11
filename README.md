# Unity-AutoGetComponent

Utility for Unity3D that adds a [GetComponent] annotation for automatically searching and setting a field

It enables you do to this:

```
public class TestAutoGetComponent : MonoBehaviour
{
    // this will be set in Awake for you
    [GetComponent] public Rigidbody rb;
}
```

And the `rb` will be set to a `Rigidbody` that is on the same GameObject.
