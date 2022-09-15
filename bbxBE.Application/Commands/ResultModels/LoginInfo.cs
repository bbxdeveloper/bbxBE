using bbxBE.Domain.Common;
using bbxBE.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace bbxBE.Application.Commands.ResultModels
{
    public class LoginInfo
    {   
        public string Token { get; set; }
        public Users User { get; set; }
    }
}
