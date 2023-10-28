using Domain.Model;
using FakeItEasy;
using Microsoft.Extensions.Logging;

namespace TokenGenerator.Test;

public class TokenGeneratorTest
{
    private readonly TokenGenerator _tokenGenerator = new TokenGenerator(
        A.Fake<ILogger<TokenGenerator>>()
    );
    
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void GenerateJwtTest()
    {
        var user = new ApplicationUser()
        {
            Id = "id",
        };
        var jwk = "{\n    \"p\": \"-z0IYQL052dPjItgVXpt423fvzZ8clYuEm7J17IyNyWZH1CsQ_EeHpGccmaS_-tBkz_GZf2paXwwXDtfOlCmgT_GXq2d_GekxjU_pm-k3TOvdMSw_Rp15P_yPOJmPgDhmg49JH27Oiaie0fYfusdruCc2AB6l3GvmJ8IoH_Hmnc\",\n    \"kty\": \"RSA\",\n    \"q\": \"wPGTKEc6nsmyeAl1QWqeDQ4nS3orblihUgeFBvXa5KMRKeN2v9cpRnkXSMh0ZnHKLm33SjYYCn_crxgFJt3ZtSsNnAx6QS7o954JN4JGK5-f6a-1ZNoSsLlkUJFCwuoElYKmhnS-X59RGPfDo3bqvv23J87CCZVJr7ptXiZMgp0\",\n    \"d\": \"mS2ejUI89xfKy1qeB_fU7kF5zKy5i5tJNmE1oT-1c0Gy06w3eOnUdN32TRf1EesB7J0SOeAIBQWjHn0JlTlH92mAtNiJpKemTHxV_fJsZtiRyXxtvfch7anMjMxjLrVzwVyK1O_Nkrs5M6i-Eti7ACeMhxfrz4L8_a31oTpFgJvjRID3Ln7A-PWrNq4F16RCpCg0wpt8k1-YylnFC7IT9wMDypFf53RIOw-dMAf6AQdpcuLtOtMisfw8hA9abonC1NqTrtWL7Pdwkg4lqCzwMbZwcuP6CKqxJU50noMspVVLSKf6O5EO5q9N8n1LA13WI0KuOySJmRcHx5hptFC1wQ\",\n    \"e\": \"AQAB\",\n    \"use\": \"sig\",\n    \"kid\": \"sig-1596744746\",\n    \"qi\": \"HYLhsuwCqnXExA5-J0vX_dgkKCUfr5LUFIRG3yFsR2GhvrlmKq2t91VtasXGnXeae7JpKoGSh20kYCerFoPomL6oraxkP_bgQP2Spmb4QtHj8MsyCvUBTzY5c9tZZsHpIGZKqCvBG3o6pvakWqNlGaF07KFdjGgzJMlavaKykQU\",\n    \"dp\": \"YrmcBkscI6aEKu1GALDoZ5twdfoaMu_MkO-6Hyll9CexIQ4Zc2VXeZmsiYPnNeujTBKeist_-_SSKFwoUokItPdzoxbEYz_zDvQzu9pZRZce1lrIVd1FAKTW8rfgh3LfNCNuQV2q6fHunVUhLNaFRof6-iJOCh7haq7ru-4aaIc\",\n    \"alg\": \"RS256\",\n    \"dq\": \"PFXO3NOZYzpTkvHyNuiuUArTAulLV5BjshEj6k9Lih_sKBs3cTpJb_5B58K3k6mVvfspT42m6vGdAywRwFuzztGNyfSk7mlK9NP4seENLGsYSK1nnggLvDPdSMyEAgvILzDR_k3XknNxmssv5Jg1RFB2y_398505k7hKpxx25Ik\",\n    \"n\": \"vVrbKTjL7vLWGhAJv0jhKgWGOtN9ZOLHzTakV967wWB89HHvQmk3Co7AqWwO4a0KqPZMXboW0OYeYgsWTOMhQ35R9u2Q9tnZFMHHftzH6mQrlZrTH6lQaXBYphMBe55bC3l49Qq2bCD4YnAwf28gr2ROQeyLaZHPj7efpzpm9mUIUskZQrhZOZO_9QdT8BrcjbHx03ZjjiZAn3_3IrJdiYAdnhWzDMoxn09zBXz11u4P6kuJo4N6k41TpGrmmVUlm0vF7T8dk3rDKdkSIDWNoNpUufR8q701HFgvLioA_04aYw0rkCOpQzgrYInF6AFd_zTBQd1t6JM6DL4zPy4o-w\"\n}";
        var token = _tokenGenerator.GenerateJwt(user, jwk);
        
        Assert.NotNull(token);;
    }

    [Test]
    public void GenerateRefreshTokenTest()
    {
        var token = _tokenGenerator.GenerateRefreshToken();
        
        Assert.IsNotEmpty(token);;
    }
    
    [Test]
    public void IsJwtExpiredTest()
    {
        var jsonWebToken =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6MTIzNDU2Nzg5LCJuYW1lIjoiSm9zZXBoIn0.OpOSSw7e485LOP5PrzScxHb7SR6sAOMRckfFwi4rp7o";
        var isExpired = _tokenGenerator.IsJwtExpired(jsonWebToken);
        
        Assert.True(isExpired);
    }
}