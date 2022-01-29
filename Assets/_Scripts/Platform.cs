using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    // should divide by 4, new platform spawns after 3/4 time
    private float lifeTime;
    private float spawnHeight;

    [SerializeField] ParticleSystem deathParticle;
    public float LifeTime { get { return lifeTime; } }

    public Vector3 size { get; private set; }
    public Vector3 center { get; private set; }

    public Coroutine platformCoroutine { get; private set; }
    public Coroutine platformMoveCoroutine { get; private set; }

    public event Action TimeToSpawnNewPlatform;
    public event Action<Vector3, Vector3> RemovePowerupsOnOldPlatform;

    private Renderer render;




    /*  FUNCTIONS  */


    private void Start()
    {
        lifeTime = Umbr.EF.Manager.GameManager.Instance.platformLife;
        spawnHeight = Umbr.EF.Manager.GameManager.Instance.platformSpawnHeight;
        render = GetComponent<Renderer>();
        BoxCollider col = gameObject.GetComponent<BoxCollider>();
        size = col.bounds.size;
        center = col.bounds.center;

        Umbr.EF.Manager.GameManager.Instance.OnPlayerCreated += Instance_OnPlayerCreated;


        if (platformCoroutine == null && Umbr.EF.Manager.GameManager.Instance.playerLogic != null)
        {
            platformCoroutine = StartCoroutine(PlatformMove());
            Umbr.EF.Manager.GameManager.Instance.playerLogic.OnPLayerDeath += PlayerLogic_OnPLayerDeath;
        }
    }

    private IEnumerator PlatformLife()
    {
        yield return new WaitForSeconds(2 * lifeTime / 4); //3*LifeTime/4
        TimeToSpawnNewPlatform?.Invoke();
        yield return new WaitForSeconds(lifeTime / 4);

        Color startColor = render.material.color;
        //yield return new WaitForSeconds(lifeTime / 4);
        for (float step = 0f; step < 1.0f; step += Time.deltaTime * 4 / LifeTime)
        {
            render.material.color = Color.Lerp(startColor, Color.red, step);
            //render.material.color = Color.Lerp(render.material.color, Color.red, step);
            yield return null;
        }

        RemovePowerupsOnOldPlatform?.Invoke(transform.position, size);
        //RemovePowerupsOnOldPlatform?.Invoke(center, size);

        Umbr.EF.Manager.GameManager.Instance.pivot.gameObject.SetActive(true);
        Umbr.EF.Manager.GameManager.Instance.pivotX.gameObject.SetActive(false);
        Umbr.EF.Manager.GameManager.Instance.pivotZ.gameObject.SetActive(false);

        Instantiate(deathParticle, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public event Action MovementComplete;
    private IEnumerator PlatformMove()
    {
        while (transform.position.y < 0)
        {
            transform.Translate(Vector3.up * Time.deltaTime * spawnHeight * 4 / lifeTime);
            yield return null;
        }
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        platformCoroutine = StartCoroutine(PlatformLife());
        MovementComplete?.Invoke();
    }






    /*  ACTIONS RESPONSES  */

    private void Instance_OnPlayerCreated()
    {
        if (platformCoroutine == null && Umbr.EF.Manager.GameManager.Instance.playerLogic != null)
        {
            platformCoroutine = StartCoroutine(PlatformLife());
            Umbr.EF.Manager.GameManager.Instance.playerLogic.OnPLayerDeath += PlayerLogic_OnPLayerDeath;
        }
    }

    private void PlayerLogic_OnPLayerDeath()
    {
        if (this != null)
            StopAllCoroutines();
    }
}

