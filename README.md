# ComponentKit

This here kit is useful if you need to model something like this:

	------------------------------------------------------------------------
	|          | Position | Health | Flying | Fire Breathing | Combustible |
	------------------------------------------------------------------------
	| "Player" |    x     |   X    |        |                |      x      |
	| "Dragon" |    x     |   X    |   x    |       x        |             |
	------------------------------------------------------------------------
	     ^                             ^
	  Unique entities      composed of arbitrary behaviors.


## Usage

Create an entity:

```C#
IEntityRecord dragon = 
    Entity.Create("A firebreathing dragon!",
        new Health()),
        new FireBreathing()));
```

You can also define entities to easily create them later:

```C#
Entity.Define("Dragon",
    typeof(Health));

Entity.Define("Player",
    typeof(Health),
    typeof(Combustible));
```

And then later:

```C#
IEntityRecord player =
    Entity.CreateFromDefinition(
        "Player", "You!");

IEntityRecord dragon =
    Entity.CreateFromDefinition(
        "Dragon", "A Firebreathing dragon!",
        new FireBreathing()));
```

When you want to manage and handle specific components separately, you set triggers. Like this:

```C#
List<Combustible> combustibles = 
    new List<Combustible>();

EntityRegistry.Current.SetTrigger(
    component => component is Combustible,
    (sender, eventArgs) => {
        foreach (IComponent component in eventArgs.Components) {
            Combustible combustible = component as Combustible;
            
            if (combustible != null) {
                if (combustible.IsOutOfSync) {
                    combustibles.Remove(combustible);
                } else {
                    combustibles.Add(combustible);
                }
            }
        }
    });
```

But don't forget to commit the changes. Otherwise the triggers **will not** be run:

```C#
EntityRegistry.Current.Synchronize();
```

### Components

They come in 2 shapes:

 - **Component**: Basic component that can be attached to any entity. Can provide behavior or just data.
 - **DependencyComponent**: Like the Component, but can specify a dependence on other components.

A very basic component could look like this:

```C#
class Health : Component {
    public Health() {
        Amount = 100;
    }

    public bool IsDead {
        get {
            return Amount <= 0;
        }
    }

    public int Amount { 
        get; 
        set; 
    }
}
```

Some components need to interact with others, take this one for example:

```C#
class Combustible : DependencyComponent {
    [RequireComponent]
    Health _health;

    public bool IsBurning { 
        get; 
        private set; 
    }

    public void Combust() {
        IsBurning = true;
    }

    public void Extinguish() {
        IsBurning = false;
    }

    public void Burn() {
        _health.Amount -= 10;
    }
}
```

Common for all components is that they have to be handled by you. The firebreathing dragon does not breathe fire until it is told to. Usually this is handled through a trigger or similar system.

```C#
class FireBreathing : DependencyComponent {
    public void Breathe(IEntityRecord target) {
        if (target.HasComponent<Combustible>()) {
            target.GetComponent<Combustible>().Combust();
        }
    }
}
```

### More

When you're ready to make stuff happen, you go like this:

```C#
dragon.GetComponent<FireBreathing>()
    .Breathe(player);
```

You could even put some real action in:

```C#
Random r = new Random();

for (int turn = 0; turn < 10; turn++) {
    foreach (Combustible combustible in combustibles) {
        if (combustible.IsBurning) {
            combustible.Burn();
        }
    }

    Combustible fire = player.GetComponent<Combustible>();
    Health condition = player.GetComponent<Health>();

    if (fire.IsBurning) {
        // There's a 10% chance that the player figures out how to extinguish himself!
        bool playerStoppedDroppedAndRolled =
            r.Next(0, 100) <= 10;

        if (playerStoppedDroppedAndRolled) {
            fire.Extinguish();
        }
    }

    if (condition.IsDead) {
    	// Unfortunately for the player, he did not figure it out in time.
        break;
    }
}
```

#### Examples

For more examples of usage, check out these repositories:

 - [shrt/Fruitless](https://github.com/shrt/Fruitless)
 - [ambient92/Calcifer](https://github.com/ambient92/Calcifer)

## License

	Copyright 2013 Jacob Hauberg Hansen.

	Permission is hereby granted, free of charge, to any person obtaining a copy of
	this software and associated documentation files (the "Software"), to deal in
	the Software without restriction, including without limitation the rights to
	use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
	of the Software, and to permit persons to whom the Software is furnished to do
	so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in all
	copies or substantial portions of the Software.

	This part is in all uppercase. 
	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
	SOFTWARE.

	http://en.wikipedia.org/wiki/MIT_License