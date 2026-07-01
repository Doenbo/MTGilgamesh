namespace MTG.DB;

//public class SQL
//{
//    private readonly string connectionString;

//    public SQL()
//    {
//        string current_path = Directory.GetCurrentDirectory();
//        string db_path = Path.GetFullPath(Path.Combine(current_path, @"..\..\..\..\MTG.DB\MTG.mdf"));
//        connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={db_path};Integrated Security=True;Connect Timeout=30";
//    }

//    public Result<ScryfallCard> GetExact(string search, string set = "")
//    {
//        var select = $@"SELECT * FROM [MTG].[Cards] WHERE Name = @Name AND Set = @Set AND CollectorNumber = @CollectorNumber";

//        using (SqlConnection connection = new SqlConnection(connectionString))
//        {
//            try
//            {
//                connection.Open();

//                var myCommand = new SqlCommand(select, connection);
//                myCommand.Parameters.AddWithValue("@Name", search);
//                var read = myCommand.ExecuteReader();


//                while (read.Read())
//                {
//                    //var name = read["Name"].ToString();
//                    //card.Name = name;
//                }

//                var card = new ScryfallCard()
//                {
//                    Id = "",
//                    Lang = "",
//                    Object = "",
//                    Layout = "",
//                    PrintsSearchUri = new Uri(""),
//                    RulingsUri = new Uri(""),
//                    ScryfallUri = new Uri(""),
//                    Uri = new Uri(""),
//                    CMC = 0,
//                    ColorIdentity = [],
//                    Keywords = [],
//                    Legalities = [],
//                    Name = "",
//                    OracleText = "",
//                    Reserved = false,
//                    TypeLine = "",
//                    Booster = false,
//                    BorderColor = "",
//                    CardBackId = "",
//                    CollectorNumber = "",
//                    Digital = false,
//                    Finishes = [],
//                    Frame = "",
//                    FullArt = false,
//                    Games = [],
//                    HighresImage = false,
//                    ImageStatus = "",
//                    Oversized = false,
//                    Prices = [],
//                    Promo = false,
//                    Rarity = "",
//                    RelatedUris = [],
//                    ReleasedAt = new DateTime(),
//                    Reprint = false,
//                    ScryfallSetUri = new Uri(""),
//                    SetName = "",
//                    SetSearchUri = new Uri(""),
//                    SetType = "",
//                    SetUri = new Uri(""),
//                    Set = "",
//                    SetId = "",
//                    StorySpotlight = false,
//                    Textless = false,
//                    Variation = false
//                };

//                return Result<ScryfallCard>.Success(card);
//            }
//            catch (Exception ex)
//            {
//                return Result<ScryfallCard>.Failure(ex.Message);
//            }
//        }
//    }

//    public Result<Card> GetFuzzy(string fuzzy, string set = "")
//    {
//        throw new NotImplementedException();
//    }

//    public Result InsertCardIntoDb(string pk, string json)
//    {
//        var insert = @$"
//                         INSERT INTO [MTG].[Cards] ([Id], {DbFieldHelper.GetInSqBrackets()})
//                         VALUES (@Id, {DbFieldHelper.GetStartingWithAt()})";

//        var merge = @$"
//                        MERGE INTO [MTG].[Cards] AS target
//                        USING (SELECT @Id, {DbFieldHelper.GetStartingWithAt()}) 
//                            AS source (Id, {DbFieldHelper.GetPlain()})
//                        ON (target.[Id] = source.Id)
//                        WHEN MATCHED THEN
//                            UPDATE SET
//                                {DbFieldHelper.GetAssigningList()}
//                        WHEN NOT MATCHED THEN
//                            INSERT ([Id], {DbFieldHelper.GetInSqBrackets()})
//                            VALUES (@Id, {DbFieldHelper.GetStartingWithAt()});";

//        using (SqlConnection connection = new SqlConnection(connectionString))
//        {
//            try
//            {
//                connection.Open();

//                SqlCommand myCommand = new SqlCommand(merge, connection);
//                //myCommand.Parameters.AddWithValue("@Id", card.Id);
//                //myCommand.Parameters.AddWithValue("@Lang", card.Lang);
//                //myCommand.Parameters.AddWithValue("@CMC", card.CMC);
//                //myCommand.Parameters.AddWithValue("@Defense", ((object)card.Defense ?? DBNull.Value)); TODO //trick if value is null
//                //myCommand.Parameters.AddWithValue("@Keywords", string.Join(", ", card.Keywords.ToArray()));
//                //myCommand.Parameters.AddWithValue("@ManaCost", Mana.ListToString(card.ManaCost));
//                //myCommand.Parameters.AddWithValue("@Name", card.Name);
//                //myCommand.Parameters.AddWithValue("@OracleText", ((object)card.OracleText ?? DBNull.Value));
//                //myCommand.Parameters.AddWithValue("@Power", ((object)card.Power ?? DBNull.Value)); TODO 
//                //myCommand.Parameters.AddWithValue("@Toughness", ((object)card.Toughness ?? DBNull.Value)); TODO
//                //myCommand.Parameters.AddWithValue("@CardTypes", string.Join(", ", card.CardTypes.ToArray()));
//                //myCommand.Parameters.AddWithValue("@SetName", card.SetName);

//                myCommand.ExecuteNonQuery();
//            }
//            catch (Exception ex)
//            {
//                return Result.Failure(ex.Message);
//            }
//        }
//        return Result.Success();
//    }
//}
