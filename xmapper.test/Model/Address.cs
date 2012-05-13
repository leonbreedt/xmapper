﻿//
// Copyright (C) 2010-2012 Leon Breedt
// ljb -at- bitserf [dot] org
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.using System;
//

using System.Collections.Generic;

namespace XMapper.Test.Model
{
    internal class Address
    {
        public string StreetName { get; set; }
        public string City { get; set; }
        public string Comments { get; set; }
        public PostCode PostCode { get; set; }
        public List<AreaCode> AreaCodes { get; set; }
    }

    internal class PostCode
    {
        public string Code { get; set; }
    }

    internal class AreaCode
    {
        public string Code { get; set; }
    }

}
