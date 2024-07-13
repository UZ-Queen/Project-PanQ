



## [0.0.2-alpha] - 2024-07-11

### Added

### Changed

- IDamageable <br>
기존의 TakeHit 메소드는 RaycastHit을 요구합니다. 적이 근접 공격을 하는 등 RayCast가 필요하지 않을 경우에 필요한 TakeDamage(int damage)를 추가했습니다.
- Projectile.cs  
적 내부에서 발사체가 생성될 경우 적을 그냥 뚫고 지나갑니다. 이를 오브젝트 생성 시 OverlapSphere를 사용해 충돌을 먼저 감지합니다.

- Projectile.cs  
![호밍샷](img/002A-HomingShot(cmp).gif)
타깃을 추적할 수 있습니다. IsHoming으로 활성화/비활성화가 가능합니다.


### TODO, Questions


## [0.0.1-alpha] - 2024-06-29



### Added
- Spawner.cs <br>
웨이브 정보(적 개수, 소환 간격)를 유니티 인스펙터 내에 저장. 

- LivingEntity.cs <br>
모든 생명체는 LivingEntity를 상속합니다.(Player, Enemy)
생명력을 가지게 되며, 0 이하로 떨어지면 죽습니다.
데미지를 입을 수 있는 IDamageable 인터페이스를 상속합니다.

- EnemyRanged.cs<br>
![원거리적](img/001alpha_Enemy(Ranged).gif) <br>
Enemy.cs 를 상속합니다. 원거리에서 총으로 공격합니다.


### Changed

- Enemy.cs<br>
![근거리적](img/alpha001_Enemy(Meele).gif)<br>
이제 플레이어를 공격합니다. 플레이어 콜라이더와 충돌했을 경우에만 데미지를 가하고, 충돌하지 않았을 경우(공격이 빗맞았을 경우) 데미지를 주지 않습니다.

- Projectile.cs <br>
 Raycast를 이용해 충돌을 판정, 데미지를 받을 수 있는 오브젝트일 경우 데미지를 줍니다. 충돌할 수 있는 오브젝트라면 오브젝트에 박힙니다. 일정 시간 후 자동 파괴됩니다.


### TODOs, Questions

- parent에 종속된 scale <br>
![보통 발사체](img/localScale%20이슈%20(3).png) <br>
![짜구라진 발사체](img/localScale%20이슈%20(2).png) <br>
scale (0.1, 0.1, 0.5)의 projectile을 scale (1,3,7)인 오브젝트의 부모로 설정할 경우 오브젝트가 찌그러지는 현상.
tranform.SetParent(object, false)를 사용해도 해결 x
부모 오브젝트의 스케일만큼 곱해졌으니 그 역수를 다시 곱하면 되지 않을까 해서 곱했지만 해결 x

``` cs
        
        transform.localScale = Vector3.Scale(transform.localScale, Utilities.GetReciprocalVector(hit.transform.localScale));
       
```
구글링을 많이 해봤는데도 해결법을 모르겠네요. 어떻게 해야 할까요?

- Enemy vs EnemyRanged <br>
Enemy 함수 내에서 Enum으로 Range, Melee 등으로 표기하고 한 클래스 내에서 관리하는게 나을지, Enemy를 상속하는 EnemyMelee, EnemyRanged 클래스를 따로 만들어서 관리하는게 편할 지 모르겠네요. 만약 Range 적의 원거리 무기의 총알이 떨어졌을 경우 Melee로 전환하는 기능을 넣고 싶다 하면 전자가 맞을 것 같고... 그게 아니면 후자로 따로 상속시키는게 맞는 지 판단이 잘 안갑니다. 후자로 상속한다 해도 대부분의 메서드들은 다 재정의해서 다시 짜야 될 것 같은데 그러면 한 클래스로 관리하는게 맞는지 잘 모르겠네요. 이럴 땐 어떻게 해야 할까요?

- 프로퍼티 vs SetValue() <br>
Projectile.cs에는 damage, velocity 변수가 있습니다.
둘 다 private이지만 velocity는 public SetVelocity()로 외부에서 접근(Gun에서 Instantiate 시 사용)하고, damage는 Damage 프로퍼티로 관리하고 있죠.
둘 다 똑같은 기능 같은데, 어떤 방법이 권장되나요?

- 오브젝트 관리: 그래픽 분리? <br>
첫 번째 질문에서 오브젝트의 스케일이 달라서 parent 시 localscale이 이상해지는 현상이 있어서 그래픽 부분을 따로 분리하는 방법에 대해서 여쭈어 보고 싶습니다.
예를 들어 Medkit 오브젝트의 콜라이더에 닿을 경우 닿은 대상의 체력을 회복합니다.
유니티에서 복잡한 모델을 다룬 적이 없어서 잘 모르겠는데, 이 경우 빈 오브젝트에 콜라이더와 스크립트를 넣고 이 오브젝트의 자식으로 그래픽 오브젝트(콜라이더는 따로 없음)를 넣으면 될까요?
