using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Unity.EditorCoroutines.Editor;

using Newtonsoft.Json;
using Proyecto26;
using set;
using card;
using UnityEditor.VersionControl;


namespace Editor
{
    public class GetCallerEditor : EditorWindow
    {
#region Private Fields
        private GUILayoutOption[] noExpandOption = { GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false) };
        private GUILayoutOption[] expandWidthOption = { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false) };
        private GUILayoutOption[] expandHeightOption = { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false) };
        private GUILayoutOption[] expandWidthHeightOption = { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true) };
        
        private RequestHelper _requestOptions;
        private ResponseHelper _myResponse;
        private MultiSetRequest _myMsr;
        private CardRequest _myCd;
        private MultiCardRequest _myMultiCd;

        private static string _host = "https://api.pokemontcg.io/v2/";
        private static string _query = "";
        private static string _page = "1";
        private static string _pageSize = "100";
        private static string _orderBy = "";
        private static string _text = "Nothing Opened...";
        private static string _queryByID = "hgss1-1";

        private static readonly string[] SearchType = {"Cards", "Sets"};
        private static int _sgSelected = 0;
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

        private static Dictionary<string, Texture> textureDict;

        private static bool _firstTime = true;

        private static int _setIndex = 0;
        private static int _cardIdx = 0;
        
        bool idFilter = true;
        bool nameFilter = true;
        bool superTypeFilter = true;
        bool subTypeFilter = true;
        bool levelFilter = true;
        bool hpFilter = true;
        bool typeFilter = true;
        bool fEvoFilter = true;
        bool tEvoFilter = true;
        bool ruleFilter = true;
        bool ancTFilter = true;
        bool ablFilter = true;
        bool atkFilter = true;
        bool wknsFilter = true;
        bool resistFilter = true;
        bool rtrCostFilter = true;
        bool convRtrCostFilter = true;
        bool setFilter = true;
        bool numFilter = true;
        bool artistFilter = true;
        bool rarityFilter = true;
        bool textFilter = true;
        bool imageFilter = true;
        bool showFilterToggle24 = true;
        bool showFilterToggle25 = true;
#endregion
        
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
                textureDict = new Dictionary<string, Texture>();
                _window = CreateInstance<GetCallerEditor>();
                _window.minSize = new Vector2(800, 750);
                _window.maxSize = new Vector2(800, 750);
                _window.titleContent.text = "pokeGET";
                _firstTime = false; }

            // Get existing open window or if none, make a new one:
            _window = (GetCallerEditor) EditorWindow.GetWindow(typeof(GetCallerEditor));
            _window.Repaint();
            _window.Show();
        }

       private void OnInspectorUpdate()
        {
            /*if (SearchType[_sgSelected] == "Card" || SearchType[_sgSelected] == "Set")
            {
                setOrCardSelected = true;
            }
            else
            {
                setOrCardSelected = false;
            }*/
        }

        private void OnGUI()
        {
           GUILayout.Space(10);
           
           
           GUILayout.BeginHorizontal(EditorStyles.helpBox);
                GUILayout.BeginVertical(EditorStyles.helpBox);
                    var orgWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 70;
                    _host = EditorGUILayout.TextField("Host", _host, GUILayout.Width(position.width * .400f));
                    if (setOrCardSelected)
                    {
                        GUILayout.Space(3);
                        _queryByID =
                            EditorGUILayout.TextField("id=", _queryByID, GUILayout.Width(position.width * .400f));
                    }
                    else
                    {
                        GUILayout.Space(3);
                        _query = EditorGUILayout.TextField("q=", _query, GUILayout.Width(position.width * .400f));
                        GUILayout.Space(3);
                        _page = EditorGUILayout.TextField("page=", _page, GUILayout.Width(position.width * .400f));
                        GUILayout.Space(3);
                        _pageSize = EditorGUILayout.TextField("pageSize=", _pageSize,
                            GUILayout.Width(position.width * .400f));
                        GUILayout.Space(3);
                        _orderBy = EditorGUILayout.TextField("orderBy=", _orderBy,
                            GUILayout.Width(position.width * .400f));
                        GUILayout.Space(5);
                    }

                    EditorGUIUtility.labelWidth = orgWidth;
                GUILayout.EndVertical();
                
                GUILayout.BeginHorizontal(EditorStyles.helpBox);
                    if (SearchType[_sgSelected] == "Cards" || SearchType[_sgSelected] == "Card")
                    {
                        GUILayout.BeginVertical(EditorStyles.helpBox); // Area between text fields and buttons for filters
                        
                            GUILayout.Label("Filters:");
                            var oldWidth = EditorGUIUtility.labelWidth;
                            GUILayout.BeginHorizontal();
                            // TODO this next section (and all of OnGUI() for that matter) need to be refactored, this is prototype level coding !
                                EditorGUIUtility.labelWidth = 1.0f;
                                idFilter = EditorGUILayout.ToggleLeft("ID#", idFilter/*, GUILayout.Width(50)*/);
                                nameFilter = EditorGUILayout.ToggleLeft("Name", nameFilter/*, GUILayout.Width(50)*/);
                                superTypeFilter = EditorGUILayout.ToggleLeft("SupT", superTypeFilter/*, GUILayout.Width(50)*/);
                                subTypeFilter = EditorGUILayout.ToggleLeft("SubT", subTypeFilter/*, GUILayout.Width(50)*/);
                                levelFilter = EditorGUILayout.ToggleLeft("Lvl", levelFilter/*, GUILayout.Width(50)*/);
                            GUILayout.EndHorizontal();
                            
                            GUILayout.BeginHorizontal();
                            {
                                hpFilter = EditorGUILayout.ToggleLeft("HP", hpFilter);
                                typeFilter = EditorGUILayout.ToggleLeft("Type", typeFilter);
                                fEvoFilter = EditorGUILayout.ToggleLeft("EvoF", fEvoFilter);
                                tEvoFilter = EditorGUILayout.ToggleLeft("EvoT", tEvoFilter);
                                ruleFilter = EditorGUILayout.ToggleLeft("Rule", ruleFilter);
                            }
                            GUILayout.EndHorizontal();
                            
                            GUILayout.BeginHorizontal();
                            {
                                ancTFilter = EditorGUILayout.ToggleLeft("AncT", ancTFilter);
                                ablFilter = EditorGUILayout.ToggleLeft("Abl", ablFilter);
                                atkFilter = EditorGUILayout.ToggleLeft("Atk", atkFilter);
                                wknsFilter = EditorGUILayout.ToggleLeft("Wkns", wknsFilter);
                                resistFilter = EditorGUILayout.ToggleLeft("Rst", resistFilter);
                            }
                            GUILayout.EndHorizontal();
                                
                            GUILayout.BeginHorizontal();
                            {
                                rtrCostFilter = EditorGUILayout.ToggleLeft("RtC", rtrCostFilter);
                                convRtrCostFilter = EditorGUILayout.ToggleLeft("cRtC", convRtrCostFilter);
                                setFilter = EditorGUILayout.ToggleLeft("Set", setFilter);
                                numFilter = EditorGUILayout.ToggleLeft("Num", numFilter);
                                artistFilter = EditorGUILayout.ToggleLeft("Arts", artistFilter);
                            }
                            GUILayout.EndHorizontal();
                            
                            GUILayout.BeginHorizontal();
                            {
                                rarityFilter = EditorGUILayout.ToggleLeft("RarT", rarityFilter);
                                textFilter = EditorGUILayout.ToggleLeft("Txt", textFilter);
                                imageFilter = EditorGUILayout.ToggleLeft("Img", imageFilter);
                                if (GUILayout.Button("Select All"))
                                {
                                    idFilter = nameFilter = superTypeFilter = subTypeFilter =
                                        levelFilter = hpFilter = typeFilter = fEvoFilter =
                                            tEvoFilter = ruleFilter = ancTFilter =
                                                ablFilter = atkFilter = wknsFilter =
                                                    resistFilter = rtrCostFilter = convRtrCostFilter =
                                                        setFilter = numFilter = artistFilter =
                                                            rarityFilter = textFilter =
                                                                imageFilter = showFilterToggle24 =
                                                                    showFilterToggle25 = true;
                                }

                                if (GUILayout.Button("None"))
                                {
                                    idFilter = nameFilter = superTypeFilter = subTypeFilter =
                                        levelFilter = hpFilter = typeFilter = fEvoFilter =
                                            tEvoFilter = ruleFilter = ancTFilter =
                                                ablFilter = atkFilter = wknsFilter =
                                                    resistFilter = rtrCostFilter = convRtrCostFilter =
                                                        setFilter = numFilter = artistFilter =
                                                            rarityFilter = textFilter =
                                                                imageFilter = showFilterToggle24 =
                                                                    showFilterToggle25 = false;
                                }
                            }
                            GUILayout.EndHorizontal();
                            EditorGUIUtility.labelWidth = oldWidth;
                        GUILayout.EndVertical();
                        
                        GUILayout.BeginVertical(); // 4 Button Column top right of screen
                            GUILayout.Space(2.75f);
                            if(GUILayout.Button("Generate SOs", expandWidthOption))
                            {
                                if (_myMultiCd != null)
                                {
                                    ExportCardsAsSO(_myMultiCd);
                                }
                            }
                            GUILayout.Space(2.75f);
                        
                            if(GUILayout.Button("Button", expandWidthOption))
                            {
                                if (_myMultiCd != null)
                                {
                                }
                            }
                            GUILayout.Space(2.75f);
                        
                        if(GUILayout.Button("Button", expandWidthOption))
                        {
                            if (_myMultiCd != null)
                            {
                            }
                        }
                        
                        GUILayout.Space(2.75f);
                        
                        if(GUILayout.Button("Button", expandWidthOption))
                        {
                            if (_myMultiCd != null)
                            {
                            }
                        }
                        
                        GUILayout.Space(2.75f);
                        
                        if(GUILayout.Button("About", expandWidthOption))
                        {
                            EditorUtility.DisplayDialog(title: "About",
                                message: "This is a button to explain this program.", "Rad");
                        }
                        
                        GUILayout.Space(2.75f);
                        
                        GUILayout.EndVertical();

                    }
                    
                    GUILayout.EndHorizontal();

                    
            GUILayout.EndHorizontal();


            GUILayoutOption[] selGridOpts = expandWidthOption;
            
            GUILayout.BeginHorizontal(EditorStyles.helpBox, selGridOpts);
            _sgSelected = GUILayout.SelectionGrid(_sgSelected, SearchType, SearchType.Length, selGridOpts);
            GUILayout.Space(5);
            
            
            if (GUILayout.Button("Submit", expandWidthOption))
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
                            
                            GUILayout.Label(_text, EditorStyles.wordWrappedLabel, textOutLayout);
                    EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            #endregion // Left output panel
                #region Right-Side Vertical Image Output Panel
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(true));
           
                switch (SearchType[_sgSelected])
                {
                    /*case "Card":
                    {
                        GUILayoutOption[] imgOutLayout = new GUILayoutOption[]{GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false), GUILayout.Width(500*0.75f), GUILayout.Height(700*0.75f)};
                        _text =  "\n" + FormatCardOutput(_myCd.data);
                        GUILayout.Label(_cardFrontTexture, imgOutLayout);
                        break;
                    }*/
                    case "Cards":
                    {
                        GUILayout.BeginHorizontal();
                            if (GUILayout.Button(string.Format("<<"))) // First Card
                            {
                                if (_myMultiCd == null || _myMultiCd.data == null) return;
                                _cardIdx = 0;
                               
                               
                                _cardFrontTexture =  RequestImage(_myMultiCd.data[_cardIdx].images.large, ImageType.Front);
                            }

                          
                            if (GUILayout.Button("<-")) // Previous Card
                            {
                                if (((_cardIdx - 1) < 0) || (_myMultiCd == null) || _myMultiCd.data == null) return;

                                --_cardIdx;
                                _text = _myMultiCd.data[_cardIdx].ToString();
                                _cardFrontTexture =  RequestImage(_myMultiCd.data[_cardIdx].images.large, ImageType.Front);

                            }

                            if (GUILayout.Button(string.Format("->"))) // Next Card
                            {
                                if (((_cardIdx + 1) >= _myMultiCd.data.Length) || (_myMultiCd == null) || _myMultiCd.data == null) return;

                                ++_cardIdx;
                                _text = _myMultiCd.data[_cardIdx].ToString();
                                
                                _cardFrontTexture = RequestImage(_myMultiCd.data[_cardIdx].images.large, ImageType.Front);
                            }
                            
                            if (GUILayout.Button(">>")) // Last Card
                            {
                                if (_myMultiCd == null || _myMultiCd.data == null) return;
                                _cardIdx = _myMultiCd.data.Length-1;
                                _text = _myMultiCd.data[_cardIdx].ToString();
                                _cardFrontTexture = RequestImage(_myMultiCd.data[_cardIdx].images.large, ImageType.Front);
                            }
                            
                            _text =  "\n" + FormatCardOutput(_myMultiCd.data[_cardIdx]);
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
                   
                    case "Sets":
                    {
                        GUILayout.BeginHorizontal();
                                if (GUILayout.Button("<-"))
                                {
                                    if (((_setIndex - 1) < 0) || (_myMsr == null) ||(_myMsr.data)== null) return;

                                    --_setIndex;
                                    _text = _myMsr.data[_setIndex].ToString();
                                    _setSymbolTexture =  RequestImage(_myMsr.data[_setIndex].images.symbol, ImageType.Symbol);
                                    _setLogoTexture =  RequestImage(_myMsr.data[_setIndex].images.logo, ImageType.Logo);
                                }

                                if (GUILayout.Button("->"))
                                {
                                    if (((_setIndex + 1) >= _myMsr.data.Length) || (_myMsr == null)) return;

                                    ++_setIndex;
                                    _text = _myMsr.data[_setIndex].ToString();
                                    _setSymbolTexture =  RequestImage(_myMsr.data[_setIndex].images.symbol, ImageType.Symbol);
                                    _setLogoTexture =  RequestImage(_myMsr.data[_setIndex].images.logo, ImageType.Logo);
                                }
                        GUILayout.EndHorizontal();
                        
                        GUILayout.BeginVertical();
                            GUILayout.BeginHorizontal(EditorStyles.helpBox);
                                    GUILayoutOption[] symbolLayout = {GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false),  GUILayout.Width(250.0f), GUILayout.Height(250.0f)};
                                  //  GUILayout.Label("Symbol");
                                    GUILayout.Label(_setSymbolTexture, symbolLayout);
                            GUILayout.EndHorizontal();
                            
                            GUILayout.BeginHorizontal(EditorStyles.helpBox);
                                GUILayoutOption[] logoLayout = {GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false),  GUILayout.Width(250.0f), GUILayout.Height(250.0f)};
                                //GUILayout.Label("Logo");
                                
                                GUILayout.Label(_setLogoTexture, logoLayout);
                                GUILayout.EndHorizontal();

                        GUILayout.EndVertical();
                        break;
                    }
                }
                
                EditorGUILayout.EndVertical();
                #endregion // End Right-Side Vertical Image Output Panel
            EditorGUILayout.EndHorizontal(); // This is the horizontal frame containing the Outputpanels
        }

        private void ExportCardsAsSO(MultiCardRequest mcReq)
        {
            int cardNum = 1;
            string path = "";// = string.Format("Assets/DeckData/HGSS1/card{0}", cardNum);
            List<LineItem> lItems = new List<LineItem>();
            foreach (var card in mcReq.data)
            {
                path = string.Format("Assets/DeckData/Decks/MindFlood/Roster/{0}.asset", card.name);
                
                lItems.Add(ScriptableObject.CreateInstance<LineItem>().Init(card.set.id, 0, card.id));
           
                AssetDatabase.CreateAsset(lItems[cardNum-1], path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = lItems[cardNum-1];
                ++cardNum;
            }

            
        }

        private void Get()
        {
            var searchTypeString = SearchType[_sgSelected].ToLower(); // initialize searchTypeString for url concatenation
           
                _requestOptions = new RequestHelper
                {
                    Uri = _host + searchTypeString,
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
            switch (v)
            {
                case "Sets": 
                {
                    _setIndex = 0;
                    _myMsr = JsonConvert.DeserializeObject<set.MultiSetRequest>(res.Request.downloadHandler.text);
                    _text += "\n" + _myMsr.data[0].ToString();
                    _setSymbolTexture =  RequestImage(_myMsr.data[0].images.symbol, ImageType.Symbol);
                    //_setSymbolTexture = _downloadedTexture;
                    GetWindow<GetCallerEditor>().Repaint();
                    _setLogoTexture =  RequestImage(_myMsr.data[0].images.logo, ImageType.Logo);
                    //_setLogoTexture = _downloadedTexture;
                    GetWindow<GetCallerEditor>().Repaint();
                    break;
                }
                case "Cards": 
                {
                    _cardIdx = 0;
                    _myMultiCd = JsonConvert.DeserializeObject<card.MultiCardRequest>(res.Request.downloadHandler.text);
                    
                    _cardFrontTexture =  RequestImage(_myMultiCd.data[0].images.large, ImageType.Front);
                    //_cardFrontTexture = _downloadedTexture;
                    GetWindow<GetCallerEditor>().Repaint();
                    break;
                }
            }
        }


        private string FormatCardOutput(CardData cDat)
        {
            string retStr = "";

            if (idFilter == true) retStr += "ID: " + cDat.id.ToString();
            if ( nameFilter == true)retStr += "\nName: " +  cDat.name.ToString();
            if ( superTypeFilter == true)retStr += "\nSupertype: " + cDat.supertype.ToString();
            if (subTypeFilter == true)
            {
                retStr += "\nSubtypes: ";
                foreach (var cDatSubtype in cDat.subtypes)
                {
                    retStr += "\n\t" + cDatSubtype;
                }

            }
            if ( levelFilter == true)
            {
                retStr += "\nLevel: ";
                if(cDat.level != null) retStr += cDat.level;
                
            }
            
            
            if ( hpFilter == true)retStr += "\nHP: " + cDat.hp;

            
            if (typeFilter == true)
            {
                retStr += "\nTypes: ";
                foreach (var typ in cDat.types)
                {
                    retStr += "\n\t" + typ;
                }
                
            }
            
            if ( fEvoFilter == true)
            {
                retStr += "\nEvolves From: ";
                if(cDat.evolvesFrom != null) retStr += cDat.evolvesFrom.ToString();
            }
            
            if (tEvoFilter == true)
            {
                retStr += "\nEvolves To: ";
                if (cDat.evolvesTo != null)
                {
                    foreach (var toPoke in cDat.evolvesTo)
                    {
                        retStr += "\n\t" + toPoke;
                    }    
                }
                
                
            }
            
            if (ruleFilter == true)
            {
                retStr += "\nRules: ";
                foreach (var rule in cDat.rules)
                {
                    retStr += "\n\t" + rule;
                }
                
            }
            if (ancTFilter == true)
            {
                retStr += "\nAncient Trait: ";
                if(cDat.ancientTrait != null) retStr += cDat.ancientTrait.ToString();
            }
            
            if (ablFilter == true)
            {
                retStr += "\nAbilities: ";
                if (cDat.abilities != null)
                {
                    foreach (var abil in cDat.abilities)
                    {
                        retStr += "\n" + abil.ToString();
                    }
                }
            }
            
            if (atkFilter == true)
            {
                retStr += "\nAttacks: ";
                if (cDat.attacks != null)
                {
                    foreach (var atk in cDat.attacks)
                    {
                        retStr += "\n" + atk.ToString();
                    }
                }
            }
            
            
           // if ( wknsFilter == true)retStr += "\n" + cDat.weaknesses.ToString();
            //if ( resistFilter == true)retStr += "\n" + cDat.resistances.ToString();
            //if ( rtrCostFilter == true)retStr += "\n" + cDat.retreatCost.ToString();
            //if (convRtrCostFilter == true) retStr +=  "\n" + cDat.convertedRetreatCost.ToString();
            if ( setFilter == true)retStr += "\n" + cDat.set.ToString();
            //if ( numFilter == true)retStr += "\n" + cDat.number.ToString();
            //if ( artistFilter == true)retStr += "\n" + cDat.artist.ToString();
            //if ( rarityFilter == true)retStr += "\n" + cDat.rarity.ToString();
            //if ( textFilter == true)retStr +=cDat.;
            if (imageFilter == true) retStr +=  "\n" + cDat.images.ToString();
            
            

            return retStr;
        }
        
        private Texture RequestImage(string url, ImageType imgT)
        {
            if (textureDict.ContainsKey(url))
            {
                return textureDict[url];
            }
            
            EditorCoroutineUtility.StartCoroutine(DownloadImage(url, imgT), this);
            
            
            return textureDict[url];

        }

        private IEnumerator DownloadImage(string url, ImageType imgT)
        {
            var request = UnityWebRequestTexture.GetTexture(url);
            yield return request.SendWebRequest();
            
            if (request.result != UnityWebRequest.Result.Success)
                Debug.Log(request.error);
            
            else
            {
                _downloadedTexture = ((DownloadHandlerTexture) request.downloadHandler).texture;
                textureDict.Add(url, _downloadedTexture);
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
            }
        }
    }
}



