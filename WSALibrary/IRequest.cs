﻿namespace SimpleWSA.WSALibrary
{
  public interface IRequest
  {
    object Send(int httpTimeout);
  }
}
