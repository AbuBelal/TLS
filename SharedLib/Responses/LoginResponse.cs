using System;
using System.Collections.Generic;
using System.Text;

namespace BaseLibrary.Responses
{
    // يرث نفس المفهوم ولكن يضيف Token و RefreshToken
    public record LoginResponse(
        bool Flag,
        string Message,
        string Token = null!,
        string RefreshToken = null!
    );
}
