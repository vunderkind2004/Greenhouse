// Clock.h

#ifndef _CLOCK_h
#define _CLOCK_h


#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif

class ClockClass
{
 protected:
	 uint32_t initMiliseconds;


 public:
	void init();

private:
//	time_t GetTimeByREST();
};

extern ClockClass Clock;

#endif

