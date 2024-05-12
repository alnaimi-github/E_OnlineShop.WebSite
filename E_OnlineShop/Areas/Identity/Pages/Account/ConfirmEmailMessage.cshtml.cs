// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace E_OnlineShop.Areas.Identity.Pages.Account
{
    public class ConfirmEmailMessageModel : PageModel
    {
     
       
        public async Task<IActionResult> OnGetAsync()
        {
         
            return Page();
        }
    }
}
