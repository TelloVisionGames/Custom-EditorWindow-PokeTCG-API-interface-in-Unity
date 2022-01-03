using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Unity.EditorCoroutines.Editor;

using Newtonsoft.Json;
using Proyecto26;
using set;
using card;


namespace Editor
{
    public class GetCallerEditor : EditorWindow
    {
        private RequestHelper _requestOptions;
        private ResponseHelper _myResponse;
        private MultiSetRequest _myMsr;
        private CardRequest _myCd;
        private MultiCardRequest _myMultiCd;

        private string _host = "https://api.pokemontcg.io/v2/";
        private string _query = "";
        private string _page = "1";
        private string _pageSize = "10";
        private string _orderBy = "";
        private string _text = "Nothing Opened...";
        private string _queryByID = "hgss1-1";

        private static readonly string[] SearchType = {"Set", "Sets", "Card", "Cards"};//, "Subtypes", "Supertypes", "Rarities"};
        private static int _sgSelected = 1;
        private static bool setOrCardSelected = false;

        private const string APIKeyValue = "b9afbddc-7eff-4954-82dd-d4ab470e94f7";
        private const string APIKeyName = "X-Api-Key";

        private static Vector2 _scrollPosition;
        
        private static GetCallerEditor _window;
        
        private Texture _cardFrontTexture;
        private Texture _setSymbolTexture;
        private Texture _setLogoTexture;
        private Texture _cardBackTexture;
        private Texture _downloadedTexture;

        private static bool _firstTime = true;

        private static int _setIndex = 0;
        private static int _cardIdx = 0;

        public GetCallerEditor(Texture downloadedTexture)
        {
            _downloadedTexture = downloadedTexture;
        }

        private enum ImageType
        {
            Logo,
            Symbol,
            Front,
            Back
        };
        
        [MenuItem("Window/pokeGET/Launch")]
        static void Init()
        {
            if (_firstTime)
            {
                _window = CreateInstance<GetCallerEditor>();
                _window.minSize = new Vector2(800, 700);
                _window.maxSize = new Vector2(900, 800);
                _window.titleContent.text = "pokeGET";
                _firstTime = false; }

            // Get existing open window or if none, make a new one:
            _window = (GetCallerEditor) EditorWindow.GetWindow(typeof(GetCallerEditor));
            _window.Repaint();
            _window.Show();
        }

        [MenuItem("Window/pokeGET/Close")]
        static void Kill()
        {
            Debug.Log("GET API window closed...");
            _window.Close();
        }

        private void OnInspectorUpdate()
        {
            if (SearchType[_sgSelected] == "Card" || SearchType[_sgSelected] == "Set")
            {
                setOrCardSelected = true;
            }
            else
            {
                setOrCardSelected = false;
            }
        }

        private void OnGUI()
        {
           GUILayout.Space(10);
           GUILayout.BeginHorizontal(EditorStyles.helpBox);
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                            _host = EditorGUILayout.TextField("Host", _host, GUILayout.ExpandWidth(true));
                            if (setOrCardSelected) 
                            {
                                GUILayout.Space(3);
                                _queryByID = EditorGUILayout.TextField("id=", _queryByID, GUILayout.Width(position.width * .50f));
                            }
                            else
                            {

                                GUILayout.Space(3);
                                _query = EditorGUILayout.TextField("q=", _query, GUILayout.ExpandWidth(true));
                                GUILayout.Space(3);
                                _page = EditorGUILayout.TextField("page=", _page, GUILayout.ExpandWidth(true));
                                GUILayout.Space(3);
                                _pageSize = EditorGUILayout.TextField("pageSize=", _pageSize, GUILayout.ExpandWidth(true));
                                GUILayout.Space(3);
                                _orderBy = EditorGUILayout.TextField("orderBy=", _orderBy, GUILayout.ExpandWidth(true));
                                GUILayout.Space(5);
                            }
                    GUILayout.EndVertical();
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    if (SearchType[_sgSelected] == "Cards" && GUILayout.Button("Import By List", GUILayout.ExpandWidth(true)))
                    {
                        string path = EditorUtility.OpenFilePanel("Overwrite with png", "", "png");
                        if (path.Length != 0)
                        {
                            Texture2D texture = Selection.activeObject as Texture2D;
                            var fileContent = File.ReadAllBytes(path);
                            texture.LoadImage(fileContent);
                        }
                    }
                    if (SearchType[_sgSelected] == "Cards" && GUILayout.Button("Other", GUILayout.ExpandWidth(true)))
                    {
                        
                    }
                    if (SearchType[_sgSelected] == "Cards" && GUILayout.Button("Other2", GUILayout.ExpandWidth(true)))
                    {
                        
                    }
                    GUILayout.EndVertical();
            GUILayout.EndHorizontal();


            GUILayoutOption[] selGridOpts = {GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(true)};
            
            GUILayout.BeginHorizontal(EditorStyles.helpBox, selGridOpts);
            _sgSelected = GUILayout.SelectionGrid(_sgSelected, SearchType, SearchType.Length, selGridOpts);
            GUILayout.Space(5);
            
            
            if (GUILayout.Button("Submit", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false)))
            {
                Get();
            }
            
            GUILayout.EndHorizontal();

         
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                #region Left Output Panel
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    _scrollPosition = EditorGUILayout.BeginScrollView(
                            _scrollPosition,
                            alwaysShowHorizontal: false,
                            alwaysShowVertical: false,
                            GUILayout.ExpandHeight(true),
                            GUILayout.ExpandWidth(false),
                            GUILayout.Width(position.width*.50f)
                            );
                 
                            GUILayoutOption[] textOutLayout = { GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(false), GUILayout.Width(position.width * .5f)};
                            //text = EditorGUILayout.TextArea(text, textOutLayout);

                            GUILayout.Label(_text, EditorStyles.wordWrappedLabel, textOutLayout);
                    EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            #endregion // Left output panel
                #region Right-Side Vertical Image Output Panel
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(true));
           
                switch (SearchType[_sgSelected])
                {
                    case "Card":
                    {
                        GUILayoutOption[] imgOutLayout = new GUILayoutOption[]{GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false), GUILayout.Width(500*0.75f), GUILayout.Height(700*0.75f)};

                        GUILayout.Label(_cardFrontTexture, imgOutLayout);
                        break;
                    }
                    case "Cards":
                    {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("<-"))
                        {
                            if (((_cardIdx - 1) < 0) || (_myMultiCd == null) || _myMultiCd.data == null) return;

                            --_cardIdx;
                            _text = _myMultiCd.data[_cardIdx].ToString();
                            RequestImage(_myMultiCd.data[_cardIdx].images.large, ImageType.Front);
                            _cardFrontTexture = _downloadedTexture;
                        }

                        if (GUILayout.Button("->"))
                        {
                            if (((_cardIdx + 1) >= _myMultiCd.data.Length) || (_myMultiCd == null) || _myMultiCd.data == null) return;

                            ++_cardIdx;
                            _text = _myMultiCd.data[_cardIdx].ToString();
                            RequestImage(_myMultiCd.data[_cardIdx].images.large, ImageType.Front);
                            _cardFrontTexture = _downloadedTexture;
                        }

                        GUILayout.EndHorizontal();
                    
                        GUILayout.BeginVertical();
                                GUILayoutOption[] frontLayout =
                                {
                                    GUILayout.Width(500*0.75f),
                                    GUILayout.Height(700*0.75f)
                                };
                                GUILayout.Label(_cardFrontTexture, frontLayout);
                        GUILayout.EndVertical();
                        break;
                    }
                    case "Set":
                    {
                        
                        GUILayoutOption[] symbolLayout =
                        {
                            GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false)};
                        GUILayoutOption[] logoLayout =
                        {
                            GUILayout.ExpandWidth(false),GUILayout.ExpandHeight(false)
                        };
                        
                        GUILayout.BeginHorizontal(EditorStyles.helpBox);                                                // Horizontal for holding "Symbol" Label and Symbol image
                                GUILayout.BeginHorizontal();                                                            // Horizontal for "symbol" label text
                                        GUILayout.Space(15);
                                        GUILayout.Label("Symbol");
                                GUILayout.EndHorizontal();

                                GUILayout.BeginHorizontal();                                                            // Horizontal for Symbol image below label
                                        GUILayout.Space(position.width * .125f);
                                        GUILayout.Label(_setSymbolTexture, symbolLayout);
                                GUILayout.EndHorizontal();
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal(EditorStyles.helpBox);                                                // Horizontal containing "Logo" Label and Logo Image
                                GUILayout.BeginHorizontal();
                                        GUILayout.Space(15);
                                        GUILayout.Label("Logo");
                                GUILayout.EndHorizontal();
                                
                                GUILayout.BeginHorizontal();
                                        GUILayout.Space(position.width * .125f);
                                        GUILayout.Label(_setLogoTexture, logoLayout);
                                GUILayout.EndHorizontal();
                        GUILayout.EndHorizontal();
                        break;
                    }
                    case "Sets":
                    {
                        GUILayout.BeginHorizontal();
                                if (GUILayout.Button("<-"))
                                {
                                    if (((_setIndex - 1) < 0) || (_myMsr == null) ||(_myMsr.data)== null) return;

                                    --_setIndex;
                                    _text = _myMsr.data[_setIndex].ToString();
                                    RequestImage(_myMsr.data[_setIndex].images.symbol, ImageType.Symbol);
                                    _setSymbolTexture = _downloadedTexture;
                                    RequestImage(_myMsr.data[_setIndex].images.logo, ImageType.Logo);
                                    _setLogoTexture = _downloadedTexture;
                                }

                                if (GUILayout.Button("->"))
                                {
                                    if (((_setIndex + 1) >= _myMsr.data.Length) || (_myMsr == null)) return;

                                    ++_setIndex;
                                    _text = _myMsr.data[_setIndex].ToString();
                                    RequestImage(_myMsr.data[_setIndex].images.symbol, ImageType.Symbol);
                                    _setSymbolTexture = _downloadedTexture;
                                    RequestImage(_myMsr.data[_setIndex].images.logo, ImageType.Logo);
                                    _setLogoTexture = _downloadedTexture;
                                }
                        GUILayout.EndHorizontal();
                        
                        GUILayout.BeginVertical();
                                GUILayoutOption[] symbolLayout =
                                {
                                    GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false), GUILayout.Height(200)
                                };
                                GUILayout.Label("Symbol");
                                GUILayout.Label(_setSymbolTexture, symbolLayout);

                                GUILayoutOption[] logoLayout =
                                {
                                    GUILayout.ExpandWidth(true), /*GUILayout.Width(200),*/ GUILayout.ExpandHeight(false),
                                    GUILayout.Height(200)
                                };
                                GUILayout.Label("Logo");

                                GUILayout.Label(_setLogoTexture, logoLayout);
                        GUILayout.EndVertical();
                        break;
                    }
                }
                
                EditorGUILayout.EndVertical();
                #endregion // End Right-Side Vertical Image Output Panel
            EditorGUILayout.EndHorizontal();
        }
        private void Get()
        {
            var searchTypeString = SearchType[_sgSelected].ToLower(); // initialize searchTypeString for url concatenation
            var formattedSearchType = searchTypeString;      // create copy of searchTypeString for next line's modification
            if (searchTypeString == "set" || searchTypeString == "card") formattedSearchType += "s"; // This line allows us to have distinct Card/Cards Set/Sets filter even though the api has no distinction

            if (searchTypeString == "set" || searchTypeString == "card") // These search types have a different UI than the others, there is only an ID field.
            {
                _requestOptions = new RequestHelper
                {
                    Uri = _host + formattedSearchType + "/" + _queryByID,
                    Method = "GET",
                    Timeout = 10,
                    Headers = new Dictionary<string, string> {{APIKeyName, APIKeyValue}},
                    ContentType = "application/json", //JSON is used by default
                    Retries = 3, //Number of retries
                    RetrySecondsDelay = 2, //Seconds of delay to make a retry
                    RetryCallback = (err, retries) => { }, //See the error before retrying the request
                    EnableDebug = true, //See logs of the requests for debug mode
                    IgnoreHttpException = true, //Prevent to catch http exceptions
                    UseHttpContinue = true,
                    RedirectLimit = 32
                };
            }
            else // This is for the other gypes of searches that use the same format {q, page, pageSize, orderBy} :: Sets, Cards, Subtypes, Supertypes, Rarities...
            {
                _requestOptions = new RequestHelper
                {
                    Uri = _host + formattedSearchType,
                    Method = "GET",
                    Timeout = 10,
                    Params = new Dictionary<string, string>
                    {
                        {"q", _query},
                        {"page", _page},
                        {"pageSize", _pageSize},
                        {"orderBy", _orderBy}
                    },
                    Headers = new Dictionary<string, string> {{APIKeyName, APIKeyValue}},
                    ContentType = "application/json", //JSON is used by default
                    Retries = 3, //Number of retries
                    RetrySecondsDelay = 2, //Seconds of delay to make a retry
                    RetryCallback = (err, retries) => { }, //See the error before retrying the request
                    EnableDebug = true, //See logs of the requests for debug mode
                    IgnoreHttpException = true, //Prevent to catch http exceptions
                    UseHttpContinue = true,
                    RedirectLimit = 32
                };
            }

            EditorCoroutineUtility.StartCoroutineOwnerless(
                HttpBase.DefaultUnityWebRequest(_requestOptions, (err, response) =>
                {
                    if (err != null)
                    {
                        EditorUtility.DisplayDialog("Error Response", err.Response, "Ok");
                    }
                    else // There was no problem == request successful!
                    {
                        _myResponse = response;
                        DeserializeBySearchType(SearchType[_sgSelected], _myResponse);
                    }
                }));
        }

        private void DeserializeBySearchType(string v, ResponseHelper res)
        {
            _text = "";
            switch (v)
            {
                case "Set": // Single Set
                {
                    var dS = JsonConvert.DeserializeObject<set.SetRequest>(res.Request.downloadHandler.text);
                    _text += dS.data.ToString();
                    RequestImage(dS.data.images.logo, ImageType.Logo);
                    _setLogoTexture = _downloadedTexture;
                    GetWindow<GetCallerEditor>().Repaint();
                    RequestImage(dS.data.images.symbol, ImageType.Symbol);
                    _setSymbolTexture = _downloadedTexture;
                    GetWindow<GetCallerEditor>().Repaint();
                    break;
                }
                case "Sets": // Multiple Sets
                {
                    _setIndex = 0;
                    _myMsr = JsonConvert.DeserializeObject<set.MultiSetRequest>(res.Request.downloadHandler.text);
                    _text += "\n" + _myMsr.data[0].ToString();
                    RequestImage(_myMsr.data[0].images.symbol, ImageType.Symbol);
                    _setSymbolTexture = _downloadedTexture;
                    GetWindow<GetCallerEditor>().Repaint();
                    RequestImage(_myMsr.data[0].images.logo, ImageType.Logo);
                    _setLogoTexture = _downloadedTexture;
                    GetWindow<GetCallerEditor>().Repaint();
                    break;
                }
                case "Card": // Single Card
                {
                    _myCd = JsonConvert.DeserializeObject<card.CardRequest>(res.Request.downloadHandler.text);
                    _text += _myCd.data.ToString();
                    RequestImage(_myCd.data.images.large, ImageType.Front);
                    _cardFrontTexture = _downloadedTexture;
                    GetWindow<GetCallerEditor>().Repaint();
                    break;
                }
                case "Cards": // Multiple Cards
                {
                    _cardIdx = 0;
                    _myMultiCd = JsonConvert.DeserializeObject<card.MultiCardRequest>(res.Request.downloadHandler.text);
                    _text += "\n" + _myMultiCd.data[0].ToString();
                    RequestImage(_myMultiCd.data[0].images.large, ImageType.Front);
                    _cardFrontTexture = _downloadedTexture;
                    GetWindow<GetCallerEditor>().Repaint();
                    break;
                }
            }
        }

        private void RequestImage(string url, ImageType imgT)
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(DownloadImage(url, imgT));
        }

        private IEnumerator DownloadImage(string url, ImageType imgT)
        {
            var request = UnityWebRequestTexture.GetTexture(url);
            yield return request.SendWebRequest();
            
            if (request.result != UnityWebRequest.Result.Success)
                Debug.Log(request.error);
            
            else
            {
                switch (imgT)
                {
                    case ImageType.Logo:
                        _setLogoTexture = ((DownloadHandlerTexture) request.downloadHandler).texture;
                        break;
                    case ImageType.Symbol:
                        _setSymbolTexture = ((DownloadHandlerTexture) request.downloadHandler).texture;
                        break;
                    case ImageType.Front:
                        _cardFrontTexture = ((DownloadHandlerTexture) request.downloadHandler).texture;
                        break;
                    case ImageType.Back:
                        _cardBackTexture = ((DownloadHandlerTexture) request.downloadHandler).texture;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(imgT), imgT, null);

                }

                GetWindow<GetCallerEditor>().Repaint();
            }
        }
    }
}



