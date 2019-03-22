﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using SocketAPI.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocketAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SendController : ControllerBase
    {
        private readonly IHubContext<Hub.Hub> _hubContext;

        public SendController(IHubContext<Hub.Hub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<ActionResult<string>> Get(string token, string msg)
        {
            await _hubContext.Clients.Group(token).SendAsync("Send", msg);
            return "OK";
        }

        /// <summary>
        /// 结局验证
        /// </summary>
        /// <param name="token"></param>
        /// <param name="control">当前控制者</param>
        /// <param name="checkerboard"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<bool> Post(string token, string control, [FromBody]List<List<ChessPieces>> checkerboard)
        {
            var current = checkerboard.SelectMany(x => x) // 当前人全部的落子
                                   .Where(x => x.Role == control);

            for (var i = 0; i < checkerboard.Count; i++)
            {
                if (current.Count(x => x.Item[0] == i) >= 5) // x轴超5子
                {
                    return true;
                }
                if (current.Count(x => x.Item[1] == i) >= 5) // y轴超5子
                {
                    return true;
                }
                if (current.Count(x => x.Item[0] - x.Item[1] == i) > 5)
                {
                    return true;
                }
                
            }
            for (var i = 0; i < checkerboard.Count*2; i++)
            {
                if (current.Count(x => x.Item[0] + x.Item[1] == i) > 5)
                {
                    return true;
                }
            }
            return false;
        }

        
    }
}
