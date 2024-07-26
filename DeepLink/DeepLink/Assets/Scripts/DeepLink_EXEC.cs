using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeepLink_EXEC : MonoBehaviour
{
    public static DeepLink_EXEC instancia { get; private set; } //instancia para rapido acesso, (_pode ser acessado por qualquer entidade para leitura, mas soh pode ser alterado pela propria classe_)

    [Tooltip("Nome do link para validar o prefixo da url")]
    public string prefixo;//A identificação do link vem aqui

    [Tooltip("Cenas válidas para serem carregadas pelo DeepLink - Insira o 'Nome' das Cena!")]//a lista de cenas que podem ser carregadas
    public List<string> cenas = new List<string>();

    [Tooltip("Parametros que podem ser extraidos do DeepLink")]//parametros extras quevamos extrair do deep link
    public Dictionary<string, string> parametros = new Dictionary<string, string>();
    
    [Tooltip("URL do DeepLink")]//A URL do deep link vem aqui
    public string url { get; private set; } = "[none]";

    //Obtem o nome da cena por meio da url 
    private string GetCenaUrl(string URL)
    {
        var uri = new System.Uri(URL);
        var Query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        return Query.Get("scene");
    }

    //Extrai os parametros adicionais da url, menos a cena
    private void ExtracaoParametros(string URL)
    {
        var uri = new System.Uri(URL);
        var Query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        foreach (var key in Query.AllKeys)
        {
            if (key != "scene")
            {
                parametros[key] = Query.Get(key);
            }
        }
    }

    //verifica se a cena desejada existe e é valida
    private bool CenaExistente(string nome)
    {
        return cenas.Contains(nome);
    }
    
    //
    private void OnDeepLinkActived(string URL)
    {
        url = URL;
        parametros.Clear();

        if (url.Contains(prefixo))
        {
            string cenaETD = GetCenaUrl(URL);
            if (CenaExistente(cenaETD))
            {
                SceneManager.LoadScene(cenaETD);
            }
            else
            {
                Debug.Log("Cena Inexistente ou invalida");
            }

            ExtracaoParametros(URL);
        }
        else
        {
            Debug.Log("A url parece não conter um prefixo válido, tente: " + prefixo);
        }
    }
    
    private void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            Application.deepLinkActivated += OnDeepLinkActived;
            if (!string.IsNullOrEmpty(Application.absoluteURL))
            {
                OnDeepLinkActived(Application.absoluteURL);
            }
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        //throw new NotImplementedException();
    }
    
}


